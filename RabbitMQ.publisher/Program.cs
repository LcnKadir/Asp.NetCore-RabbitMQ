using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

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
            properties.Persistent = true;

            var product = new Product { Id = 1, Name = "Stapler", Stock = 3, Price = 20 };
            var prdocutJsonString = JsonSerializer.Serialize(product);


            chanell.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(prdocutJsonString));
            
            
            Console.WriteLine("mesaj gönderilmiştir.");


            Console.ReadLine();

        }
    }
}
