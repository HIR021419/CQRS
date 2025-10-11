using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using UserService.DataAccess;

namespace UserService.Services;

public class IntegrationEventSenderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private CancellationTokenSource _wakeupCancelationTokenSource = new CancellationTokenSource();
    public IntegrationEventSenderService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        using var scope = _scopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<UserServiceContext>();
        dbContext.Database.EnsureCreated();
    }
    public void StartPublishingOutstandingIntegrationEvents()
    {
        _wakeupCancelationTokenSource.Cancel();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await PublishOutstandingIntegrationEvents(stoppingToken);
        }
    }
    private async Task PublishOutstandingIntegrationEvents(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            var connection = await factory.CreateConnectionAsync();
            var channelOpts = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true
            );
            var channel = await connection.CreateChannelAsync(channelOpts);
            //channel.ConfirmSelect(); // enable publisher confirms
            BasicProperties props = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent
            };
            while (!stoppingToken.IsCancellationRequested)
            {
                {
                    using var scope = _scopeFactory.CreateScope();
                    using var dbContext =
                        scope.ServiceProvider.GetRequiredService<UserServiceContext>();
                    var events = dbContext.IntegrationEventOutbox.OrderBy(o => o.ID).ToList();
                    foreach (var e in events)
                    {
                        var body = Encoding.UTF8.GetBytes(e.Data);
                        await channel.BasicPublishAsync(exchange: "user",
                            routingKey: e.Event,
                            body: body,
                            basicProperties: props,
                            mandatory: true
                        );
                        Debug.WriteLine("Published: " + e.Event + " " + e.Data);
                        dbContext.Remove(e);
                        dbContext.SaveChanges();
                    }
                }
                using var linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(_wakeupCancelationTokenSource.Token, stoppingToken);
                try
                {
                    await Task.Delay(Timeout.Infinite, linkedCts.Token);
                }
                catch (OperationCanceledException)
                {
                    if (_wakeupCancelationTokenSource.Token.IsCancellationRequested)
                    {
                        Debug.WriteLine("Publish requested");
                        var tmp = _wakeupCancelationTokenSource;
                        _wakeupCancelationTokenSource = new CancellationTokenSource();
                        tmp.Dispose();
                    }
                    else if (stoppingToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("Shutting down.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.ToString());
        }
    }
}
