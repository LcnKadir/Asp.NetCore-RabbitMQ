

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Runtime.CompilerServices;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        public static string ExchangeName = "ImageDirectExhange";
        public static string RoutingWatermark = "watermark-route--image";
        public static string QueueName = "queue-watermak-image";

        public readonly ILogger<RabbitMQClientService> _logger;


        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            

            if (_channel is {IsOpen:true})
            {
                return _channel;
            }

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, type: "direct", true, false);
            _channel.QueueDeclare(QueueName, true, false,false,null);
            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingWatermark);
            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu.");

            return _channel;

        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Dispose();
            _connection?.Close();

            _logger.LogInformation("RabbitMQ ile bağlantı koptu...");
        }
    }
}
