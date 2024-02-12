using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

namespace RabbitMQWeb.ExcelCreate.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _clienService;

        public RabbitMQPublisher(RabbitMQClientService clienService)
        {
            _clienService = clienService;
        }


        public void Publish(CreateExcelMessage createExcelMessage)
        {
            var channel = _clienService.Connect();
            var bodystring = JsonSerializer.Serialize(createExcelMessage);
            var bodybyte = Encoding.UTF8.GetBytes(bodystring);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingExcel, basicProperties: properties, body: bodybyte);

        }
    }
}
