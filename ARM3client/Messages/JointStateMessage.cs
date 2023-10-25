using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Messages
{
    public class JointStateMessage : ReceivedMessage
    {
        public bool init;
        public long steps_to_back;
        public long full_steps;
        public long position;
        public double speed;
        public JointStateMessage(int num, bool init, long steps_to_back, long full_steps, long position, double speed) : base(num)
        {
            this.init = init;
            this.position = position;
            this.speed = speed;
            this.full_steps = full_steps;
            this.steps_to_back = steps_to_back;
        }
    }
}
