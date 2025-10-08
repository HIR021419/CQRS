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

        public UserEventListener(IServiceScopeFactory scopeFactory)
        {
            _serviceScopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory() { 
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
            _connection = await factory.CreateConnectionAsync(CancellationToken.None);
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);
            await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "");

            try
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += Consumer_Received;
                await _channel.BasicConsumeAsync(queue: QueueName, autoAck: true,consumer: consumer);
                _channel.CallbackExceptionAsync += _channel_CallbackExceptionAsync;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var serviceScope = _serviceScopeFactory.CreateScope();
            TodoDbContext context = serviceScope.ServiceProvider.GetRequiredService<TodoDbContext>();

            var body = @event.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var eventType = @event.RoutingKey;

            var user = JsonSerializer.Deserialize<User>(message);

            if (user == null) return;

            var existing = context!.Users.FirstOrDefault(u => u.Id == user.Id);
            if (existing == null && eventType == "user.add") context.Users.Add(user);
            else if (existing != null && eventType == "user.update") existing.Name = user.Name;

            await context.SaveChangesAsync();
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
