using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Messages
{
    internal class UnknownMessage : ReceivedMessage
    {
        public string Message { get; set; }
        public UnknownMessage(string message) : base(0)
        {
            Message = message;
        }
    }
}
