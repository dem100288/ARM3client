using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Messages
{
    internal class MoveEnd : ReceivedMessage
    {
        public MoveEnd(int num) : base(num)
        {
        }
    }
}
