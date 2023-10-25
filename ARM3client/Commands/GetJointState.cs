using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    public class GetJointState : ArmCommand
    {
        public GetJointState(int num) 
        {
            Name = $"s{num}";
        }
    }
}
