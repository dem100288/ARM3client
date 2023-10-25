using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    internal class SetBackToZeroJointCommand : ArmCommand
    {
        public SetBackToZeroJointCommand(int num, long steps)
        {
            Name = $"Z{num} {steps.ToString().PadLeft(5, '0')}";
        }
    }
}
