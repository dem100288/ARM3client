using ARM3client.Commands;
using ARM3client.Helpers.Quaternions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ARM3client.Arm.Joints
{
    public abstract class Joint : ArmPart
    {
        public delegate void HandleEventTargetChanged();
        public event HandleEventTargetChanged EventTargetChanged;

        public Vector3 Mark { get; set; }
        public Segment SegmentBefore { get; set; }
        public Segment SegmentAfter { get; set; }
        public Joint? JointBefore { get; set; }
        public Joint? JointAfter { get; set; }
        public bool Rotated { get; set; } = false;
        public bool Position { get; set; } = false;

        private float _maxValue;
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    PropertyChangedEventCall(nameof(MaxValue));
                }
            }
        }
        private float _minValue = 0;
        public float MinValue
        {
            get => _minValue;
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    PropertyChangedEventCall(nameof(MinValue));
                }
            }
        }
        public bool BlockAxis { get; set; } = false;

        public Vector3 Axis { get; set; }
        public virtual Vector3 AxisReverse => Vector3.Negate(Axis);
        public virtual Vector3 GlobalAxis => Vector3.Transform(Axis, BeforePart?.GlobalRotation ?? Quaternion.Identity);
        public virtual Vector3 GlobalAxisReverse => Vector3.Transform(AxisReverse, AfterPart?.GlobalRotationReverse ?? Quaternion.Identity); //может не правильно

        public abstract float Value { get; set; }
        public float StoredValue { get; set; }


        public int NumJoint { get; set; }


        private double _speed = 0;
        public double Speed
        {
            get => _speed;
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    PropertyChangedEventCall(nameof(Speed));
                }
            }
        }
        public float RangeValue => MaxValue - MinValue;
        public float ValuePerStep => RangeValue / MaxSteps;
        public long StepsBackToZero { get; set; } = 0;
        private long _curentSteps;

        public long CurentSteps
        {
            get => _curentSteps;
            set
            {
                if (_curentSteps != value)
                {

                    _curentSteps = value;
                    Value = MinValue + _curentSteps * ValuePerStep;
                    PropertyChangedEventCall(nameof(CurentSteps));
                    PropertyChangedEventCall(nameof(Value));
                }
            }
        }
        public long TargetSteps { get; set; } = 0;
        public long ZeroValueInSteps { get; set; }
        public long MaxSteps { get; set; }
        public float NormalValue { get; set; }
        //public float CurrentValue { get; set; }
        public bool isRunning { get; set; } = false;
        private bool _isInit = false;
        public bool isInit
        {
            get => _isInit;
            set
            {
                if (_isInit != value)
                {
                    _isInit = value;
                    PropertyChangedEventCall(nameof(isInit));
                }
            }
        }

        public void ClearTraget()
        {
            SetTargetValue(StoredValue);
        }

        public void SafeValue()
        {
            StoredValue = Value;
        }

        public void FindSegments()
        {
            SegmentBefore = GetBeforeSegment() ?? throw new Exception("Not find before segment");
            SegmentAfter = GetAfterSegment() ?? throw new Exception("Not find before segment");
        }

        public void FindJoints()
        {
            JointBefore = GetBeforeJoint();
            JointAfter = GetAfterJoint();
        }

        public Joint(ArmModel model, int num, float minValue, float maxValue, long maxSteps, float normalAngle, long stepsBackToZero, Vector3 axis, Vector3 mark) :base(model)
        {
            Axis = axis == Vector3.Zero ? axis : Vector3.Normalize(axis);
            Mark = Vector3.Normalize(mark);
            MinValue = minValue;
            MaxValue = maxValue;
            NumJoint = num;
            MaxSteps = maxSteps;
            NormalValue = normalAngle;
            ZeroValueInSteps = Convert.ToInt64(MathF.Round(MathF.Abs(MinValue) / ValuePerStep));
            StepsBackToZero = stepsBackToZero;
            SetTargetValue(normalAngle);
        }

        public virtual void SetTargetValue(float targetValue)
        {
            float target = float.Clamp(targetValue, MinValue, MaxValue);
            Value = target;
            TargetSteps = ZeroValueInSteps + Convert.ToInt64(Math.Round(target / ValuePerStep));
            EventTargetChanged?.Invoke();
        }

        protected virtual (Vector3 vb, Vector3 va, Vector3 norm) GetVectorsToRotation()
        {
            return (Vector3.Transform(Mark, JointAfter?.GlobalRotationReverse ?? Quaternion.Identity), Vector3.Transform(Mark, JointBefore?.GlobalRotation ?? Quaternion.Identity), GlobalAxisReverse);
        }

        protected virtual (Vector3 vb, Vector3 va, Vector3 norm) GetVectorsToPosition()
        {
            return (SegmentAfter.LocalLocationWithRotation, SegmentAfter.EndPointReverse - SegmentBefore.GlobalLocation, GlobalAxis);
        }

        public abstract void ApproachPosition();
        public abstract void ApproachRotation();
        //public abstract void ForwardToTarget();

        public ArmCommand Init(bool force = false)
        {
            return new FindHomeCommand(NumJoint, force);
        }
    }
}
