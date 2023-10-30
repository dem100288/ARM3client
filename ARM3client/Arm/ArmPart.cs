using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.ComponentModel;
using System.Windows.Controls;
using ARM3client.Helpers.Quaternions;
using System.Reflection.Metadata;
using ARM3client.Helpers.Quaternions.Matrix;
using ARM3client.Arm.Joints;

namespace ARM3client.Arm
{
    public abstract class ArmPart : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void PropertyChangedEventCall(string? PropertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        protected ArmModel _model;

        public ArmPart(ArmModel model)
        {
            _model = model;
        }

        public ArmPart? BeforePart { get; set; }
        public ArmPart? AfterPart { get; set; }
        public Segment? GetBeforeSegment() => (BeforePart as Segment) ?? BeforePart?.GetBeforeSegment();
        public Segment? GetAfterSegment() => (AfterPart as Segment) ?? AfterPart?.GetAfterSegment();
        public Joint? GetBeforeJoint() => (BeforePart as Joint) ?? BeforePart?.GetBeforeJoint();
        public Joint? GetAfterJoint() => (AfterPart as Joint) ?? AfterPart?.GetAfterJoint();

        public virtual Vector3 GlobalLocation => (BeforePart?.GlobalLocation ?? Vector3.Zero) + LocalLocationWithRotation;
        public virtual Vector3 LocalLocation => Vector3.Zero;
        public virtual Vector3 LocalLocationWithRotation => Vector3.Transform(LocalLocation, GlobalRotation);
        public virtual Quaternion LocalRotation => Quaternion.Identity;
        public virtual Vector3 LocalRotationAngles => LocalRotation.GetEulerAngles();
        public virtual Quaternion GlobalRotation => (BeforePart?.GlobalRotation ?? Quaternion.Identity) * LocalRotation;
        public virtual Vector3 GlobalRotationAngles => GlobalRotation.GetEulerAngles();

        public virtual Vector3 GlobalLocationReverse => (AfterPart?.GlobalLocationReverse ?? Vector3.Zero) + LocalLocationWithRotationReverse;
        public virtual Vector3 LocalLocationReverse => Vector3.Zero;
        public virtual Vector3 LocalLocationWithRotationReverse => Vector3.Transform(LocalLocationReverse, GlobalRotationReverse);
        public virtual Quaternion LocalRotationReverse => Quaternion.Identity;
        public virtual Quaternion GlobalRotationReverse => (AfterPart?.GlobalRotationReverse ?? Quaternion.Identity) * LocalRotationReverse;
    }
}
