using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Messages
{
    internal class BlockMoveEnd : ReceivedMessage
    {
        public BlockMoveEnd(int num) : base(num)
        {
        }
    }
}
