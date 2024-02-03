using RabbitMQ.Client;
using System;
using System.Globalization;
using System.Text;

namespace RabbitMQ.publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://localhost:5672");

           using var connection = factory.CreateConnection();

            var chanell = connection.CreateModel();

            chanell.QueueDeclare("hello-queue", true, false, false);

            string message = "Hello World!";

            var messageBody = Encoding.UTF8.GetBytes(message);

            chanell.BasicPublish(string.Empty, "hello-queue", null, messageBody);

            Console.WriteLine("Mesaj gönderilmiştir");

            Console.ReadLine();


        }
    }
}
