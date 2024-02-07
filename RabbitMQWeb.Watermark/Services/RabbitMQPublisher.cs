using Microsoft.AspNetCore.Components;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _clienService;

        public RabbitMQPublisher(RabbitMQClientService clienService)
        {
            _clienService = clienService;
        }


        public void Publish(ProductImageCreatedEvent productImageCreatedEvent) 
        {
            var channel = _clienService.Connect();
            var bodystring = JsonSerializer.Serialize(productImageCreatedEvent);
            var bodybyte = Encoding.UTF8.GetBytes(bodystring);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingWatermark, basicProperties: properties, body: bodybyte);
        
        }
    }
}
