using RabbitMQ.Client;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RabbitMQ.publisher
{

    public enum LogNames
    {
        Critial=1,
        Error=2,
        Warning=3,
        Info=4
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

            chanell.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Fanout);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var routkey = $"route-{x}";
                var queueName = $"direct-queue-{x}";
                chanell.QueueDeclare(queueName, true, false, false);

                chanell.QueueBind(queueName, "logs-direct", routkey, null);
            });



            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log = (LogNames) new Random().Next(1,5);

                string message = $"log-type: {log}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                var routkey = $"route-{log}";


                chanell.BasicPublish("logs-direct", routkey, null, messageBody);

                Console.WriteLine($"Log gönderilmiştir: {message}");

            });


            Console.ReadLine();

        }
    }
}
