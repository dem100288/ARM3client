using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Messages
{
    public class PongMessage : ReceivedMessage
    {
        public PongMessage(int num) : base(num)
        {
        }
    }
}
