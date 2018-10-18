using log4net;
using log4net.Config;
using RabbitMQ.Client;
using Receive.Services;
using System;
using System.IO;
using System.Reflection;
using System.Text;
 

namespace Receive
{
    class Receiver
    {
        public static void Main()
        {
            

            
            Console.WriteLine("Please enter Queue Name");
            //var queuename = Console.ReadLine();
            var queuename = "efada_certificates";//
            Console.WriteLine("---------------------Now Reading Messages-------------------");

            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var queueDeclareResponse = channel.QueueDeclare(queue: queuename, durable: true, exclusive: false, autoDelete: false, arguments: null);
                        var consumer = new QueueingBasicConsumer(channel);
                        channel.BasicConsume(queue: queuename, autoAck: false, consumer: consumer);
                        for (int i = 0; i < queueDeclareResponse.MessageCount; i++)
                        {
                            var ea = consumer.Queue.Dequeue();
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] Received {0}", message);
                            //Saving to database
                            MessageLogger.AddLogs(Guid.NewGuid(), message);
                            //Set Acknowledgement to true
                            channel.BasicAck(ea.DeliveryTag, true);
                        }
                        Console.WriteLine("Finished processing {0} messages.", queueDeclareResponse.MessageCount);
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
