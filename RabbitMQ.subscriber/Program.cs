using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
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

            var randomQueue = chanell.QueueDeclare().QueueName;

            chanell.QueueBind(randomQueue, "logs-fanout", "", null);



            chanell.BasicQos(0, 1, false);

            var cunsormer = new EventingBasicConsumer(chanell);


            var queueName = "direct-queue-Critial";
            chanell.BasicConsume(queueName, false, cunsormer);


            Console.WriteLine("Loglar dinleniyor...");


            cunsormer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);

                Console.WriteLine("Gelen Mesaj:"+message);

                File.AppendAllText("log-critical.txt", message+"\n");

                chanell.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }

    }
}
