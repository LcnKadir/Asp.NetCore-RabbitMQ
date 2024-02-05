using RabbitMQ.Client;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RabbitMQ.publisher
{

    public enum LogNames
    {
        Critial = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }



    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://localhost:5672"); //works with docker
            //factory.Uri = new Uri("amqps://inrnmhyu:iMdQiUhib6hEWobcG_GkWyGvD4a5pamJ@cougar.rmq.cloudamqp.com/inrnmhyu");

            using var connection = factory.CreateConnection();

            var chanell = connection.CreateModel();

            chanell.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);


            Random rnd = new Random();
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log1 = (LogNames)rnd.Next(1, 5);
                LogNames log2 = (LogNames)rnd.Next(1, 5);
                LogNames log3 = (LogNames)rnd.Next(1, 5);
                

                var routkey = $"{log1}.{log2}.{log3}";
                string message = $"log-type: {log1}-{log2}-{log3}";
                var messageBody = Encoding.UTF8.GetBytes(message);
                chanell.BasicPublish("logs-topic", routkey, null, messageBody);

                Console.WriteLine($"Log gönderilmiştir: {message}");

            });


            Console.ReadLine();

        }
    }
}
