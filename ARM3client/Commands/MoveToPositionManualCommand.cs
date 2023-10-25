using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    public class MoveToPositionManualCommand : MoveToPositionMaxSpeedCommand
    {
        public MoveToPositionManualCommand(long joint1, long joint2, long joint3, long joint4, long joint5, long joint6) : base(joint1, joint2, joint3, joint4, joint5, joint6)
        {
            Name = "MM";
        }
    }
}
