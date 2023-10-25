using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    public class FindHomeCommand : ArmCommand
    {
        public FindHomeCommand(bool force = false)
        {
            Name = (force ? "F" : "f") + "H";
        }

        public FindHomeCommand(int joint, bool force = false)
        {
            Name = (force ? "F" : "f") + joint.ToString();
        }
    }
}
