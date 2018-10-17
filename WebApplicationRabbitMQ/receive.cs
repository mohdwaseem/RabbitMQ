using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Receive
{
    class Receiver
    {
        public static void Main()
        {
            
            Console.WriteLine("Please enter Queue Name");
            var queuename = Console.ReadLine();
            Console.WriteLine("---------------------Now Reading Messages-------------------");
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var queueDeclareResponse = channel.QueueDeclare(queue: queuename, durable: true,exclusive: false,autoDelete: false,arguments: null);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queue: queuename, autoAck: false, consumer: consumer);

                     
                    for (int i = 0; i < queueDeclareResponse.MessageCount; i++)
                    {
                        if (i%2==0)
                        {
                            var ea = consumer.Queue.Dequeue();
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] Received {0}", message);
                            channel.BasicAck(ea.DeliveryTag, true);
                            
                        }
                       
                    }
                    Console.WriteLine("Finished processing {0} messages.", queueDeclareResponse.MessageCount);
                    Console.ReadLine();
                }
            }
        }
    }
}
