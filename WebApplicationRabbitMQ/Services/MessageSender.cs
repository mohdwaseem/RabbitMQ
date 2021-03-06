﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApplicationRabbitMQ.Services
{
    public class MessageSender : IMessageSender
    {
        /// <summary>
        /// Send Single Message
        /// </summary>
        /// <param name="msg"></param>
        public void SendSingleMessage(string msg,string queue)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
                var body = Encoding.UTF8.GetBytes(msg);
                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
            }
        }
        /// <summary>
        /// Method to Receive MSG
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public string ReceiveSingleMessage(string queue)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
                var message = "---Reading----";
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                     message = Encoding.UTF8.GetString(body);
                    
                };
                channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

                return message;
            }
        }
        public void SendMessage(string msg)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = msg;

                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: "task_queue",
                                     basicProperties: properties,
                                     body: body);
                //Console.WriteLine(" [x] Sent {0}", message);
            }


        }
        public string ReceiveMessage()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                //Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                var data = "[*] Waiting for messages.";
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                   // Console.WriteLine(" [x] Received {0}", message);
                    data += string.Format("[x] Received {0}{1}", message, Environment.NewLine);
                    //int dots = message.Split('.').Length - 1;
                    //Thread.Sleep(dots * 1000);

                   // Console.WriteLine(" [x] Done");
                    data += string.Format("[x]Done {0}", Environment.NewLine);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "task_queue",
                                     autoAck: true,
                                     consumer: consumer);
                return data;
                 
            }
        }
    }
}
