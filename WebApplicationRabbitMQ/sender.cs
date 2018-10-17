using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;

namespace Send
{
    class NewTask
    {
        public static void Main(string[] args)
        {
            //Get Queue Name from Appsettings.json file
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var queue = configuration["queue"];

            //Create Connection
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
                
            using (var channel = connection.CreateModel())//Creating Channel(model)
            {
                channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;


                for (int i = 0; i < 15; i++)
                {
                    var message = "Message: "+i;//GetMessage();
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: properties,
                                     body: body);
                }

                Console.WriteLine(" [x] Message Sent");
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage()
        {

            JObject jsonObj = JObject.Parse(File.ReadAllText(@"sampleData.json"));

            return jsonObj.ToString();
        }
    }
}


///{
  "type": "test1",
  "data": {
    "FirstName": "Zamin",
    "LastName": "Khan",
    "Location": "Z2 W11",
    "Email": "mwaseem@elm.sa",
    "Ext": "3011"
  }
}

///
{
  "exchange": "my test value 1",
  "queue": "efada_certificates"

}