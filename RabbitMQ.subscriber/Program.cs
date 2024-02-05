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
            


            chanell.BasicQos(0, 1, false);
            var cunsormer = new EventingBasicConsumer(chanell);


            var queueName = chanell.QueueDeclare().QueueName;
            //var routkey = "*.Error.*";
            //var routkey = "*.*.Warning";            
            var routkey = "Info.#";
            chanell.QueueBind(queueName, "logs-topic", routkey);


            chanell.BasicConsume(queueName, false, cunsormer);


            Console.WriteLine("Loglar dinleniyor...");


            cunsormer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);

                Console.WriteLine("Gelen Mesaj:" + message);

                File.AppendAllText("log-critical.txt", message + "\n");

                chanell.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }

    }
}
