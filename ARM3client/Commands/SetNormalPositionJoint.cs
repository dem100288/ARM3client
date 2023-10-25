using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    internal abstract class SetNormalPositionJoint : ArmCommand
    {
        public SetNormalPositionJoint(int num, long steps) {
            Name = $"N{num} {steps.ToString().PadLeft(5, '0')}";
        }
    }
}
