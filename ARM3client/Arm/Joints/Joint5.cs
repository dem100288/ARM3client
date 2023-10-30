using ARM3client.Helpers.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Arm.Joints
{
    public class Joint5 : RotatedJointRotate
    {
        public Joint5(ArmModel model, int num, float minValue, float maxValue, long maxSteps, float normalAngle, long stepsBackToZero, Vector3 axis, Vector3 mark) 
            : base(model, num, minValue, maxValue, maxSteps, normalAngle, stepsBackToZero, axis, mark)
        {
        }

        protected override (Vector3 vb, Vector3 va, Vector3 norm) GetVectorsToRotation()
        {
            return (SegmentBefore.LocalLocationWithRotationReverse, /*_model.PositionPart.LastPoint*/Vector3Helper.AddedLengthZ(_model.PositionPart.LastPoint, GlobalLocationReverse) - GlobalLocationReverse, GlobalAxisReverse);
        }
    }
}
