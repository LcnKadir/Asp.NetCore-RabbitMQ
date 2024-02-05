using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

            chanell.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("formats", "pdf");
            headers.Add("shape2", "a4"); 
            //headers.Add("shape", "a4");

            var properties = chanell.CreateBasicProperties();
            properties.Headers = headers;

            chanell.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));
            
            
            Console.WriteLine("mesaj gönderilmiştir.");


            Console.ReadLine();

        }
    }
}
