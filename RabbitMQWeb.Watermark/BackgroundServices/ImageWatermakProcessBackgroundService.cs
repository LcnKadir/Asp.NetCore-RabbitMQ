using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQWeb.Watermark.Services;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQWeb.Watermark.BackgroundServices
{
    public class ImageWatermakProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ILogger<ImageWatermakProcessBackgroundService> _logger;
        private IModel _channel;
        public ImageWatermakProcessBackgroundService(RabbitMQClientService rabbitMQClientService, ILogger<ImageWatermakProcessBackgroundService> logger)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();

            _channel.BasicQos(0, 1, false); //RabbitMQ kaçar kaçar dağıtsın. //Let RabbitMQ run away and distribute it.

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cunsomer = new AsyncEventingBasicConsumer(_channel);
           
            cunsomer.Received += Cunsomer_Received;
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, cunsomer);
            return Task.CompletedTask;

        }

        private Task Cunsomer_Received(object sender, BasicDeliverEventArgs @event)
        {

            try
            {
                var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", productImageCreatedEvent.ImageName);

                var sitename = "www.WatermarkImage.com";

                using var img = Image.FromFile(path);
                using var grapich = Graphics.FromImage(img);
                var font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold, GraphicsUnit.Pixel);

                var textsize = grapich.MeasureString(sitename, font);


                var color = Color.FromArgb(128, 255, 255, 255);
                var bruhs = new SolidBrush(color);

                var position = new Point(img.Width - ((int)textsize.Width + 30), img.Height - ((int)textsize.Height + 30));


                grapich.DrawString(sitename,font,bruhs,position);
                img.Save("wwwroot/Images/watermarks/" + productImageCreatedEvent.ImageName);

                img.Dispose();
                grapich.Dispose();

                _channel.BasicAck(@event.DeliveryTag, false);

            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex.Message);
            }      

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
