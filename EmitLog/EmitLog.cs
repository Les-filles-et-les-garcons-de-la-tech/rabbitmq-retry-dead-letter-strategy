using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;

class EmitLog
{
    public static void Main(string[] args)
    {

        var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
        factory.UserName = "writer";
        factory.Password = "writer";
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            var main_queue = "main-queue";
            var main_queue2 = "main-queue2";
            var main_exchange = "main-exchange";
     

            var routingKey = main_queue;
            var routingKey2 = main_queue2;
            var message =  "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);
            var message2 = "Hello World number 2!";
            var body2 = Encoding.UTF8.GetBytes(message2);
            IBasicProperties properties = channel.CreateBasicProperties();           
            // Les messages sont persistents.
            properties.DeliveryMode = 2;
            channel.BasicPublish(exchange: main_exchange, routingKey: routingKey, basicProperties: properties, body: body);
            Console.WriteLine(" [{0}] Envoie vers '{1}' du message '{2}'", main_exchange, routingKey, message);
                        channel.BasicPublish(exchange: main_exchange, routingKey: routingKey2, basicProperties: null, body: body2);
            Console.WriteLine(" [{0}] Envoi vers '{1}' du message '{2}'", main_exchange, routingKey2, message2);
        }
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}