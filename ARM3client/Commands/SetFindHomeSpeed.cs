using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    internal class SetFindHomeSpeed : ArmCommand
    {
        public SetFindHomeSpeed(int speed) {
            Name = $"SF {speed.ToString().PadLeft(5, '0')}";
        }
    }
}
