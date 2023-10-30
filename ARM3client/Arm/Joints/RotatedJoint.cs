using ARM3client.Helpers.Quaternions;
using ARM3client.Helpers.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Arm.Joints
{
    public class RotatedJoint : Joint
    {
        public RotatedJoint(ArmModel model, int num, float minValue, float maxValue, long maxSteps, float normalAngle, long stepsBackToZero, Vector3 axis, Vector3 mark)
            : base(model, num, minValue, maxValue, maxSteps, normalAngle, stepsBackToZero, axis, mark)
        {
        }

        private float Angle { get; set; } = 0;
        public override float Value
        {
            get => Angle;
            set
            {
                Angle = value;
            }
        }

        public override Quaternion LocalRotation => Quaternion.CreateFromAxisAngle(Axis, Angle * Vector3Helper.DegresToRadiansKoef);



        public override Quaternion LocalRotationReverse => Quaternion.CreateFromAxisAngle(AxisReverse, Angle * Vector3Helper.DegresToRadiansKoef);

        public override void ApproachPosition() => RotateToTargetPosition();
        public override void ApproachRotation() => RotateToTargetRotation();

        public virtual void RotateToTargetRotation()
        {
            var vectors = GetVectorsToRotation();
            var pvb = vectors.vb.Projection(vectors.norm);
            var pva = vectors.va.Projection(vectors.norm);
            var ang = AngleToRotate(pvb, pva, vectors.norm);
            var new_anlge = ang + Value;
            if (MathF.Abs(MathF.Abs(new_anlge) - MathF.Abs(Value)) > 90)
            {
                new_anlge = 180 - MathF.Abs(new_anlge);
            }
            var angle = float.Clamp(new_anlge, MinValue, MaxValue);
            if (MathF.Abs(angle - Value) > 0)
            {
                SetTargetValue(angle);
            }
        }

        private float AngleToRotate(Vector3 v1, Vector3 v2, Vector3 normal)
        {
            var angle = v1.AngleBetweenVectorsDegres(v2);
            
            //var angle = MathF.Abs(a - MathF.Abs(Value));
            if (!float.IsNaN(angle) && !float.IsInfinity(angle) && angle > ValuePerStep)
            {
                var pvbn = Vector3.Normalize(v1);
                var pvan = Vector3.Normalize(v2);
                var q = Quaternion.CreateFromAxisAngle(normal, angle * Vector3Helper.DegresToRadiansKoef);
                var plvnt = Vector3.Transform(pvbn, q);
                var plvnr = plvnt - pvan;
                var sang = plvnr.Length() < 0.01f ? 1 : -1;
                var ang = angle * sang;

                return ang;
            }
            return 0f;
        }

        public virtual void RotateToTargetPosition()
        {
            var vectors = GetVectorsToPosition();
            var pvb = vectors.vb.Projection(vectors.norm);
            var pva = vectors.va.Projection(vectors.norm);
            var angle = float.Clamp(AngleToRotate(pvb, pva, vectors.norm) + Value, MinValue, MaxValue);
            if (MathF.Abs(angle - Value) > 0)
            {
                SetTargetValue(angle);
            }
        }
    }
}
