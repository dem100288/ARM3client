using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    public class SetMaxSpeedCommand : ArmCommand
    {
        public SetMaxSpeedCommand(int max_speed)
        {
            Name = $"SS {max_speed.ToString().PadLeft(5, '0')}";
        }
    }
}
