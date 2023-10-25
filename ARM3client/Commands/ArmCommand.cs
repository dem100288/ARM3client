using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    public class ArmCommand
    {
        public string Name { get; set; } = "  ";
        public virtual string ToStringCommand()
        {
            return $"{Name}";
        }
    }
}
