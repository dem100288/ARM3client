﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    public class GetJointsState : ArmCommand
    {
        public GetJointsState() 
        {
            Name = "st";
        }
    }
}
