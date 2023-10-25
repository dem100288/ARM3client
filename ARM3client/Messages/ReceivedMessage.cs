using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ARM3client.Messages
{
    public class ReceivedMessage
    {
        public int NumJoint { get; set; }
        public ReceivedMessage(int num)
        {
            NumJoint = num;
        }
    }
}
