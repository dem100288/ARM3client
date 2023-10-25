using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    internal class TestLimmitJoint : ArmCommand
    {
        public TestLimmitJoint(int joint)
        {
            Name = "t" + joint.ToString();
        }
    }
}
