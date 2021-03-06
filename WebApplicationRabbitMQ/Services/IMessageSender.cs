﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationRabbitMQ.Services
{
   public interface IMessageSender
    {
        void SendMessage(string msg);
        string ReceiveMessage();
        void SendSingleMessage(string msg, string queue);
        string ReceiveSingleMessage(string queue);
    }
}
