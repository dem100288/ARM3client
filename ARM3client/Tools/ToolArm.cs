using ARM3client.Arm;
using ARM3client.Helpers.Quaternions;
using ARM3client.Helpers.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Tools
{
    public class ToolArm : Segment
    {
        public ToolArm(ArmModel model, Vector3 direct, Vector3? axis = null, float angle = 0) : base(model, direct)
        {
            _normalRotation = axis is null ? Quaternion.Identity : Quaternion.CreateFromAxisAngle(axis ?? Vector3.Zero, angle * Vector3Helper.DegresToRadiansKoef);
            _normalRotationReverse = axis is null ? Quaternion.Identity : Quaternion.CreateFromAxisAngle(Vector3.Negate(axis ?? Vector3.Zero), angle * Vector3Helper.DegresToRadiansKoef);
            //_targetRotationReverse = Quaternion.Identity;
            _targetRotation = Quaternion.Identity;
            
        }

        public virtual Vector3 TargetVector { get; set; }
        private Quaternion _targetRotation { get; set; }
        private Quaternion _normalRotation { get; set; }
        //private Quaternion _targetRotationReverse { get; set; }
        private Quaternion _normalRotationReverse { get; set; }
        public virtual Quaternion TargetRotation => _targetRotation;// * _normalRotation;

        public override Quaternion LocalRotation => _normalRotation;
        public override Quaternion LocalRotationReverse => _targetRotation * _normalRotationReverse;

        public override Vector3 GlobalLocationReverse => TargetVector;
        public override Quaternion GlobalRotationReverse => LocalRotationReverse;

        //public void SetTargetRotation(float x, float y, float z)
        //{
        //    SetTargetRotation(Quaternion.CreateFromAxisAngle(Vector3.UnitX, x * Vector3Helper.DegresToRadiansKoef)
        //        * Quaternion.CreateFromAxisAngle(Vector3.UnitY, y * Vector3Helper.DegresToRadiansKoef)
        //        * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, z * Vector3Helper.DegresToRadiansKoef));
        //}

        public void SetTargetRotation(Quaternion rotation)
        {

            _targetRotation = rotation;
        }

        //public void SetTargetRotation(Vector3 vect)
        //{
            
        //    SetTargetRotation(vect.X, vect.Y, vect.Z);
        //}
    
    }
}
