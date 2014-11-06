using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NEXMI
{
    public class UserMessage
    {
        public String Sender { get; set; }
        public String Receiver { get; set; }
        public String Message { get; set; }

        public UserMessage()
        {
        }
        public UserMessage(String sender, String receiver, String message)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.Message = message;
        }
    }
}