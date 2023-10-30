using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Arm
{
    public class ArmBase : Segment
    {
        public ArmBase(ArmModel model, Vector3 direct, Quaternion? rotation = null) : base(model, direct)
        {
            globalRotation = rotation ?? Quaternion.Identity;
        }

        private Quaternion globalRotation;
        public override Quaternion GlobalRotation => globalRotation;
    }
}
