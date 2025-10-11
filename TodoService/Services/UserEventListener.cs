using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using TodoService.DataAccess;
using TodoService.Models;

namespace TodoService.Services
{
    public class UserEventListener : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        private const string ExchangeName = "user";
        private const string QueueName = "todo_user_queue";
        private const string HostName = "rabbitmq";

        public UserEventListener(IServiceScopeFactory scopeFactory)
        {
            _serviceScopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory() { HostName = HostName };

            int retryCount = 0;
            const int maxRetryDelay = 30000;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(cancellationToken);
                    _channel = await _connection.CreateChannelAsync();
                    Debug.WriteLine("Connecté à RabbitMQ");
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    int delay = Math.Min(1000 * retryCount, maxRetryDelay);
                    Debug.WriteLine($"Échec de connexion à RabbitMQ (tentative {retryCount}) : {ex.Message}. Nouvelle tentative dans {delay / 1000}s.");
                    await Task.Delay(delay, cancellationToken);
                }
            }

            if (_connection == null || _channel == null)
            {
                Debug.WriteLine("Impossible d'établir la connexion à RabbitMQ.");
                return;
            }

            await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);
            await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "");

            try
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += Consumer_Received;
                await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);
                _channel.CallbackExceptionAsync += _channel_CallbackExceptionAsync;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la configuration du consommateur : {ex}");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var serviceScope = _serviceScopeFactory.CreateScope();
            TodoServiceContext context = serviceScope.ServiceProvider.GetRequiredService<TodoServiceContext>();

            var body = @event.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var eventType = @event.RoutingKey;

            Console.WriteLine(message);
            var user = JsonSerializer.Deserialize<User>(message, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (user == null) return;

            var existing = context!.Users.FirstOrDefault(u => u.Id == user.Id);
            if (existing == null && eventType == "user.add") context.Users.Add(user);
            else if (existing != null && eventType == "user.update")
            {
                if (user.Version > existing.Version)
                {
                    existing.Name = user.Name;
                    existing.Version = user.Version;
                    context.Users.Update(existing);
                }
            }

            await context.SaveChangesAsync();
            await _channel.BasicAckAsync(@event.DeliveryTag, false);
        }

        private Task _channel_CallbackExceptionAsync(object sender, CallbackExceptionEventArgs @event)
        {
            Debug.WriteLine(@event.Exception.ToString());
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            _channel?.CloseAsync(cancellationToken);
            _connection?.CloseAsync(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
