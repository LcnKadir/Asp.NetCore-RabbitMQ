using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace RabbitMQ.subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://localhost:5672");

            using var connection = factory.CreateConnection();

            var chanell = connection.CreateModel();
            chanell.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);


            chanell.BasicQos(0, 1, false);
            var cunsormer = new EventingBasicConsumer(chanell);


            var queueName = chanell.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("formats", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "any"); //Verilerin içinde bir tane istenilen olsa bile veriyi çeker. //It pulls the data even if there is only one desired one in the data. 
            //headers.Add("x-match", "all"); //Gönderilen verilerin bire bir aynısı olması gerekir. //The data sent must be identical.


            chanell.QueueBind(queueName, "header-exchange", String.Empty, headers);


            chanell.BasicConsume(queueName, false, cunsormer);


            Console.WriteLine("Loglar dinleniyor...");


            cunsormer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Product product = JsonSerializer.Deserialize<Product>(message);

                Thread.Sleep(1500);

                Console.WriteLine($"Gelen Mesaj: {product.Id}-{product.Name}-{product.Stock}-{product.Price}");

                chanell.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }

    }
}
