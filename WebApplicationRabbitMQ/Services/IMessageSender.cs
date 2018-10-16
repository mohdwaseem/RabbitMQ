using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationRabbitMQ.Services
{
   public interface IMessageSender
    {
        string SendMessage(string msg);
    }
}
