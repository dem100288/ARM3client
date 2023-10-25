using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Messages
{
    internal class JointPosition : ReceivedMessage
    {
        
        public long Position { get; set; }
        public JointPosition(int num, long position) :base(num)
        {
            Position = position;
        }
    }
}
