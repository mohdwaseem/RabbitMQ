using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationRabbitMQ.Services
{
    public class MessageSender : IMessageSender
    {
        public string SendMessage(string msg)
        {
            return "This is the message from DI container";
        }
    }
}
