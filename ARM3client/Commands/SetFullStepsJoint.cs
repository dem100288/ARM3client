using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    internal class SetFullStepsJoint : ArmCommand
    {
        public SetFullStepsJoint(int num, long steps) {
            Name = $"S{num} {steps.ToString().PadLeft(5, '0')}";
        }
    }
}
