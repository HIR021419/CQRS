using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace UserService.Services
{
    public class MessagePublisher
    {
        private readonly ConnectionFactory _factory = new ConnectionFactory() { 
            HostName = "rabbitmq",
            UserName = "guest",
            Password = "guest"
        };
        private IConnection? _connection = null;
        private IChannel? _channel = null;
        private const string ExchangeName = "user";

        private static MessagePublisher? Instance = null;

        public static MessagePublisher GetInstance()
        {
            if (MessagePublisher.Instance == null) Instance = new MessagePublisher();
            return Instance;
        }

        public async Task Publish(string integrationEvent, object data)
        {
            string eventData = JsonSerializer.Serialize(data);

            if (_connection == null) _connection = await _factory.CreateConnectionAsync();
            if (_channel == null) { 
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);
            }
            var body = Encoding.UTF8.GetBytes(eventData);
            await _channel.BasicPublishAsync(exchange: ExchangeName, routingKey: integrationEvent, body: body);
        }
    }
}
