using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

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

            //chanell.QueueDeclare("hello-queue", true, false, false);

            var cunsormer = new EventingBasicConsumer(chanell);

            chanell.BasicConsume("hello-queue", true, cunsormer);

            cunsormer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Gelen Mesaj:"+message);
            };

            Console.ReadLine();
        }

    }
}
