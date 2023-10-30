using ARM3client.Helpers.Quaternions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Arm
{
    public class Segment : ArmPart
    {
        public virtual float Length { get; set; } = 0;
        public Vector3 Direct { get; set; }
        
        public Segment(ArmModel model, Vector3 direct) :base(model)
        {
            Length = direct.Length();
            Direct = direct == Vector3.Zero ? direct : Vector3.Normalize(direct);
        }

        public override Vector3 LocalLocation => Direct * Length;
        //public override Vector3 GlobalLocation => (BeforePart?.GlobalLocation ?? Vector3.Zero) + LocalLocation;
        public virtual Vector3 StartPoint => GlobalLocation - LocalLocationWithRotation;
        public virtual Vector3 EndPoint => GlobalLocation;

        public override Vector3 LocalLocationReverse => Vector3.Negate(LocalLocation);
        //public override Vector3 GlobalLocationReverse => (AfterPart?.GlobalLocation ?? Vector3.Zero) + LocalLocationReverse;
        public virtual Vector3 StartPointReverse => GlobalLocationReverse;
        public virtual Vector3 EndPointReverse => GlobalLocationReverse - LocalLocationWithRotationReverse;
    }
}
