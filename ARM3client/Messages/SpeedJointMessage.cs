using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Messages
{
    internal class SpeedJointMessage : ReceivedMessage
    {
        public double Speed { get; set; }
        public SpeedJointMessage(int num, double speed) : base(num)
        {
            Speed = speed;
        }
    }
}
