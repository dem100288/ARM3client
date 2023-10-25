using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Messages
{
    internal class JointFindHome : ReceivedMessage
    {
        public JointFindHome(int num) : base(num)
        {
        }
    }
}
