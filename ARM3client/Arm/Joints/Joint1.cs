using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Arm.Joints
{
    public class Joint1 : RotatedJointPosition
    {
        public Joint1(ArmModel model, int num, float minValue, float maxValue, long maxSteps, float normalAngle, long stepsBackToZero, Vector3 axis, Vector3 mark) 
            : base(model, num, minValue, maxValue, maxSteps, normalAngle, stepsBackToZero, axis, mark)
        {
        }

        protected override (Vector3 vb, Vector3 va, Vector3 norm) GetVectorsToPosition()
        {
            return (SegmentAfter.LocalLocationWithRotation, _model.RotationPart.PointToPosition, GlobalAxis);
        }
    }
}
