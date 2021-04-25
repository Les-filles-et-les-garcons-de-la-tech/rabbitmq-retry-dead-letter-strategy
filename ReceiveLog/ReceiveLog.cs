using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Collections.Generic;

class ReceiveLogs
{
    public static void Main(string[] args)
    {

        var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
        factory.UserName = "consumer";
        factory.Password = "consumer";

        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            var queue_name = "main-queue";
            var dead_letter_exchange= "dead-letter-exchange";
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                Console.WriteLine(" [{0}] Received {1} ", queue_name, message);
                IDictionary<string, object>  headers = ea.BasicProperties.Headers;
                var time = DateTime.Now.ToString("HH:mm:ss tt");
                var xDeathCount = ExtractXDeathCount(headers);
                try
                {                 
                    //###################################################
                    // ...... Le message est traité                     # 
                    //###################################################

                    // Tout se passe bien on le ACK pour le supprimer
                    var MAX_RETRY = 10;
                    if(xDeathCount < MAX_RETRY){
                        Console.WriteLine(" [{0}] nombre de retry: {1} - time: {2} pour l'instant tout va bien ", queue_name, xDeathCount, time);
                        channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                    // Si le nombre de retry maximum est atteint on envoie, pour le stocker, 
                    // un message avec le même body que l'original vers la dead-letter-queue via le dead-letter-queue
                    else {
                        Console.WriteLine(" [{0}] nombre de retry: {1} - time: {2}, on va te mettre de côté", queue_name, xDeathCount, time);
                        channel.BasicPublish(exchange: dead_letter_exchange, routingKey: queue_name, basicProperties: null, body: ea.Body);
                        // Le message original est ACK pour le supprimer
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        Console.WriteLine(" [{0}] Envoie vers '{1}':'{2} '", queue_name, dead_letter_exchange, message);
                    }
                }
                catch (Exception)
                {
                    var MAX_RETRY = 10;
                    if(xDeathCount < MAX_RETRY){
                        Console.WriteLine(" [{0}] nombre de retry: {1} - time: {2} pour l'instant tout va bien ", queue_name, xDeathCount, time);
                        channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                    // Si le nombre de retry maximum est atteint on envoie, pour le stocker, 
                    // un message avec le même body que l'original vers la dead-letter-queue via le dead-letter-queue
                    else {
                        Console.WriteLine(" [{0}] nombre de retry: {1} - time: {2}, on va te mettre de côté", queue_name, xDeathCount, time);
                        channel.BasicPublish(exchange: dead_letter_exchange, routingKey: queue_name, basicProperties: null, body: ea.Body);
                        // Le message original est ACK pour le supprimer
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        Console.WriteLine(" [{0}] Envoie vers '{1}':'{2} '", queue_name, dead_letter_exchange, message);
                    }
                }
            };
            channel.BasicConsume(queue: queue_name, autoAck: false, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    private static long ExtractXDeathCount(IDictionary<string, object> headers)
    {
        long xDeathCount = 0;
        if (headers != null ) {
            var xDeath = ((List<object>) headers["x-death"])[0];
            IDictionary<string, object>  xDeathDictonnary = (IDictionary<string, object>) xDeath;
            xDeathCount = (long) xDeathDictonnary["count"];
            }
        return xDeathCount;
    }      
}