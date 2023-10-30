using ARM3client.Arm.Joints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Arm
{
    public class RotationPart
    {
        public List<Joint> Joints { get; set; } = new();
        public IEnumerable<Joint> JointsReverse => Joints.Reverse<Joint>();
        public Vector3 PointToPosition => Joints.First().GlobalLocationReverse;

        private ArmModel _model;
        public RotationPart(ArmModel model)
        {
            _model = model;
        }

        public void AddJoint(Joint joint)
        {
            Joints.Add(joint);
        }

        public void AddRange(IEnumerable<Joint> joints)
        {
            Joints.AddRange(joints);
        }

        public void ApproachToTarget()
        {
            foreach (var joint in JointsReverse)
            {
                if (!(joint.GlobalAxis == Vector3.Zero || joint.BlockAxis))
                {
                    joint.ApproachRotation();
                }
            }
        }
    }
}
