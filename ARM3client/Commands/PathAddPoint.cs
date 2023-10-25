using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    public  class PathAddPoint : ArmCommand
    {
        
        public PathAddPoint(float speed, IEnumerable<long> joints)
        {
            Name = $"PA {Convert.ToInt32(MathF.Round(speed)).ToString().PadLeft(5, '0')} {string.Join(' ', joints.Select(x => x.ToString().PadLeft(5, '0')))}";
        }
    }
}
