using ARM3client.Arm.Joints;
using ARM3client.Commands;
using ARM3client.Helpers.Quaternions;
using ARM3client.Helpers.Quaternions.Matrix;
using ARM3client.Helpers.Vectors;
using ARM3client.Messages;
using ARM3client.PathManage;
using ARM3client.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ARM3client.Arm
{
    public class ArmModel : INotifyPropertyChanged
    {
        public static ArmModel CreateAR3AnninRobotics(Dispatcher disp)
        {

            var model = new ArmModel(2000, disp);
            model.AddBase(new ArmBase(model, Vector3.Zero))
                .AddJoint(new Joint1(model, 1, -123.75f, 123.75f, 44000, 0, 7600, new Vector3(0, 0, -1), new Vector3(0, -1, 0)))
                .AddSegment(new Segment(model, new Vector3(0, 64.20f, 169.77f)))
                .AddJoint(new Joint2(model, 2, -36f, 81f, 52000, 0, 2000, new Vector3(-1, 0, 0), new Vector3(0, -1, 0)))
                .AddSegment(new Segment(model, new Vector3(0, 0, 305)))
                .AddJoint(new Joint3(model, 3, -136.8f, 46.8f, 40800, -90, 800, new Vector3(1, 0, 0), new Vector3(0, -1, 0)))
                
                .AddSegment(new Segment(model, new Vector3(0, 0, 222.63f)))
                .AddJoint(new Joint4(model, 4, -160, 160, 52000, 0, 2600, new Vector3(0, 0, 1), new Vector3(0, -1, 0)))
                .AddJoint(new Joint5(model, 5, -94.5f, 94.5f, 8400, 0, 200, new Vector3(-1, 0, 0), new Vector3(0, -1, 0)))
                .AddJoint(new Joint6(model, 6, -137.4f, 137.4f, 23200, 0, 2000, new Vector3(0, 0, -1), new Vector3(0, -1, 0)))
                .AddSegment(new Segment(model, new Vector3(0, 0, 36.25f)))
                //.AddSegment(new Segment(new Vector3(0, 0, 0)))
                .SetTool(new ToolArm(model, Vector3.Zero, new Vector3(1, 0, 0), 90));
            model.SetVariables();
            return model;
        }

        public ConnectionPort _port;
        public ArmBase Base { get; set; }
        public List<Joint> Joints { get; set; } = new List<Joint>();
        public List<Segment> Segments { get; set; } = new List<Segment>();
        public List<ArmPart> Parts { get; set; } = new List<ArmPart>();
        public PositionPart PositionPart { get; set; }
        public RotationPart RotationPart { get; set; }
        public ToolArm Tool { get; set; }
        public ToolBox ToolBox { get; set; }
        public ObservableCollection<string> InCom => _port.InCommands;
        public ObservableCollection<string> OutCom => _port.OutCommands;

        public bool ManualMove { get; set; } = false;
        public bool isMoving { get; set; } = false;

        private float MinValueChange = -5f;
        private float MaxValueChange = 5f;

        public Vector3 _positionEndPoint { get; set; }
        public Vector3 _rotationEndPoint { get; set; }

        public Vector3 TargetPositionEndPoint { get; set; }
        public Quaternion TargetRotation { get; set; }
        public Vector3 TargetRotationEndPoint => TargetRotation.GetEulerAngles();

        public float PX => PositionEndPoint.X; 
        public float PY => PositionEndPoint.Y;
        public float PZ => PositionEndPoint.Z;
        public Vector3 BaseReverse;
        public float BX => BaseReverse.X;
        public float BY => BaseReverse.Y;
        public float BZ => BaseReverse.Z;
        public Vector3 PositionEndPoint 
        {
            get => _positionEndPoint;
            set
            {
                _positionEndPoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PX)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PY)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PZ)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BX)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BY)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BZ)));
            }
        }

        public float RX => RotationEndPoint.X;
        public float RY => RotationEndPoint.Y;
        public float RZ => RotationEndPoint.Z;
        public Vector3 RotationEndPoint
        {
            get => _rotationEndPoint;
            set
            {
                _rotationEndPoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RX)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RY)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RZ)));
            }
        }


        public int FindHomeSpeed { get; set; } = 500;

        public bool ArmIsInit => Joints.All(x => x.isInit);
        private bool ProcessInitRun = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        private ArmModel(int findHomeSpeed, Dispatcher disp)
        {
            _port = new ConnectionPort(disp: disp);
            FindHomeSpeed = findHomeSpeed;
            _port.OnReceiveMessage += _port_OnReceiveMessage;
            PositionPart = new PositionPart(this);
            RotationPart = new RotationPart(this);
            ToolBox = new ToolBox();
        }

        public void ClearTarget()
        {
            foreach (var joint in Joints)
            {
                joint.ClearTraget();
            }
        }

        public void SafeValueJoints()
        {
            foreach (var joint in Joints)
            {
                joint.SafeValue();
            }
        }

        public void MovePath(PathTool path)
        {
            _port.SendCommand(new PathClear());
            foreach (var point in path.Path)
            {
                _port.SendCommand(new PathAddPoint(point.Speed, point.JointValues));
            }
            _port.SendCommand(new PathMove());
        }

        public bool CalcBackwardTask(Vector3 target, Vector3 rortation, bool moveNow = true) => CalcBackwardTask(target, Vector3Helper.VectorToQuaternion(rortation), moveNow);

        public bool CalcBackwardTask(Vector3 target, Quaternion rotation, bool moveNow = true)
        {
            SafeValueJoints();
            var delta = 1;
            Tool.TargetVector = target;
            Tool.SetTargetRotation(rotation);
            var d = (Tool.TargetVector - Tool.GlobalLocation).Length();

            var tool_e = Tool.GlobalRotation.GetEulerAngles();
            var targ_e = Tool.TargetRotation.GetEulerAngles();
            

            var dr = (tool_e - targ_e).Length();
            int i = 0;
            bool find_pos = (d < delta && dr < delta);
            while (!find_pos && i < 100)
            {
                //if (dr > d) {
                RotationPart.ApproachToTarget();
                //} else {
                PositionPart.ApproachToTarget();
                //}
                tool_e = Tool.GlobalRotation.GetEulerAngles();
                targ_e = Tool.TargetRotation.GetEulerAngles();
                dr = (tool_e - targ_e).Length();
                d = (Tool.TargetVector - Tool.GlobalLocation).Length();
                find_pos = (d < delta && dr < delta);
                var j1 = Joints[0].Value;
                var j2 = Joints[1].Value;
                var j3 = Joints[2].Value;
                var j4 = Joints[3].Value;
                var j5 = Joints[4].Value;
                var j6 = Joints[5].Value;
                i++;
            }
            if (find_pos)
            {
                if (moveNow) MoveToTarget();
                return true;
            }
            else
            {
                ClearTarget();
                return false;
            }
        }

        public void MoveToTarget()
        {
            _port.SendCommand(new MoveToPositionEqAllJointCommand(Joints[0].TargetSteps, Joints[1].TargetSteps, Joints[2].TargetSteps, Joints[3].TargetSteps, Joints[4].TargetSteps, Joints[5].TargetSteps));
            isMoving = true;
        }

        public void WaitMoveEnd()
        {
            while (isMoving) { Thread.Sleep(1); }
        }

        public void MoveToZero()
        {
            foreach (var joint in Joints)
            {
                joint.SetTargetValue(joint.NormalValue);
            }
            Tool.SetTargetRotation(QuaternionHelper.GetQuaternionFromAngles(0, 0, 0));
            MoveToTarget();
        }

        private void SetVariables()
        {
            _port.SendCommand(new SetFindHomeSpeed(FindHomeSpeed));
            foreach (var joint in Joints)
            {
                _port.SendCommand(new SetBackToZeroJointCommand(joint.NumJoint, joint.StepsBackToZero));
                _port.SendCommand(new SetFullStepsJoint(joint.NumJoint, joint.MaxSteps));
                joint.FindSegments();
                joint.FindJoints();
                //_port.SendCommand(new SetNormalPositionJoint(joint.NumJoint, joint.NormalPosition));
                joint.EventTargetChanged += Joint_EventTargetChanged;
            }
            _port.SendCommand(new GetJointsState());
            PositionPart.AddRange(Joints.Where(x => x.Position));
            RotationPart.AddRange(Joints.Where(x => x.Rotated));
        }

        private void Joint_EventTargetChanged()
        {
            ArmCommand? com = null;
            if (ManualMove)
            {
                MoveToTarget();
            }
            //else
            //    com = new MoveToPositionManualCommand(Joints[0].TargetSteps, Joints[1].TargetSteps, Joints[2].TargetSteps, Joints[3].TargetSteps, Joints[4].TargetSteps, Joints[5].TargetSteps);
            
        }

        private void UpdatePosRotate()
        {
            var pos_rot = ForwardKinematics();
            PositionEndPoint = pos_rot.position;
            RotationEndPoint = pos_rot.rotation;
            Tool.SetTargetRotation(QuaternionHelper.GetQuaternionFromAngles(RotationEndPoint));
            Tool.TargetVector = PositionEndPoint;
            BaseReverse = Base.GlobalLocationReverse;
        }

        private void _port_OnReceiveMessage(ReceivedMessage message)
        {
            switch (message)
            {
                case FindHome FH:
                    {
                        //Joints[0].TargetSteps = 0;
                        //Joints[1].TargetSteps = 0;
                        //Joints[2].TargetSteps = 0;
                        //Joints[3].TargetSteps = 0;
                        //Joints[4].TargetSteps = 0;
                        //Joints[5].TargetSteps = 0;
                        //var com = new MoveToPositionEqAllJointCommand(Joints[0].NormalPosition, Joints[1].NormalPosition, Joints[2].NormalPosition, Joints[3].NormalPosition, Joints[4].NormalPosition, Joints[5].NormalPosition);
                        //_port.SendCommand(com);
                        MoveToZero();
                        break;
                    }
                case JointFindHome JFH:
                    {
                        Joints[JFH.NumJoint - 1].isInit = true;
                        InitProc();
                        break;
                    }
                case JointPosition JP:
                    {
                        Joints[JP.NumJoint - 1].CurentSteps = JP.Position;
                        break;
                    }
                case MoveEnd ME:
                    {
                        Joints[0].CurentSteps = Joints[0].TargetSteps;
                        Joints[1].CurentSteps = Joints[1].TargetSteps;
                        Joints[2].CurentSteps = Joints[2].TargetSteps;
                        Joints[3].CurentSteps = Joints[3].TargetSteps;
                        Joints[4].CurentSteps = Joints[4].TargetSteps;
                        Joints[5].CurentSteps = Joints[5].TargetSteps;
                        
                        UpdatePosRotate();
                        if (ManualMove) ManualMove = false;
                        isMoving = false;
                        break;
                    }
                case BlockMoveEnd BME:
                    {

                        break;
                    }
                case UnknownMessage UnknownMessage:
                    {
                        //TODO
                        break;
                    }
                case PongMessage pong:
                    {
                        //TODO
                        break;
                    }
                case SpeedJointMessage speedJoint:
                    {
                        Joints[speedJoint.NumJoint - 1].Speed = speedJoint.Speed;
                        break;
                    }
                case JointStateMessage state: 
                    {
                        var joint = Joints[state.NumJoint - 1];
                        joint.isInit = state.init;
                        joint.Speed = state.speed;
                        joint.CurentSteps = state.position;
                        joint.TargetSteps = state.position;
                        if (ArmIsInit) UpdatePosRotate();
                        //joint.StepsBackToZero = state.steps_to_back;
                        //joint.MaxSteps = state.full_steps;


                        break; 
                    }
                default:
                    {
                        //TODO
                        break;
                    }
            }
        }

        public void ArmInit()
        {
            ProcessInitRun = true;
            _port.SendCommand(new FindHomeCommand(true));
            //InitProc();
        }

        public void Ping()
        {
            _port.SendCommand(new PingCommand());
        }

        private void InitProc()
        {
            if (ProcessInitRun)
            {
                if (!ArmIsInit)
                {
                    //var joint = Joints.FirstOrDefault(x => !x.isInit);
                    //if (joint is not null)
                    //{
                    //    _port.SendCommand(joint.Init());
                    //}
                }
                else
                {
                    ProcessInitRun = false;
                }
            }
        }

        

        public ArmModel AddJoint(Joint joint)
        {
            var last = Parts.Last();
            joint.BeforePart = last;
            last.AfterPart = joint;
            Joints.Add(joint);
            Parts.Add(joint);

            return this;
        }

        public ArmModel SetTool(ToolArm tool) 
        {
            if (Tool is not null)
            {
                Parts.Remove(Tool);
            }
            Tool = tool;
            
            return AddSegment(tool); 
        }

        public ArmModel AddBase(ArmBase @base)
        {
            if (Base is not null)
            {
                Parts.Remove(@base);
            }
            Base = @base;

            return AddSegment(@base);
        }

        public ArmModel AddSegment(Segment segment)
        {
            var last = Parts.LastOrDefault();
            segment.BeforePart = last;
            if (last is not null)
            {
                last.AfterPart = segment;
            }
            Parts.Add(segment);
            Segments.Add(segment);
            return this;
        }

        public ArmModel AddPart(ArmPart part)
        {
            if (part is Joint joint)
            {
                return AddJoint(joint);
            }
            else if (part is Segment segment)
            {
                return AddSegment(segment);
            }
            else if (part is ToolArm tool)
            {
                return SetTool(tool);
            }
            return this;
        }

        

        public (Vector3 position, Vector3 rotation) ForwardKinematics()
        {
            //Vector3 prevPoint = Joints[0].StartOffset;
            //Quaternion rotation = Quaternion.Identity;
            //for (int i = 1; i < Joints.Count; i++)
            //{
                
            //    // Выполняет поворот вокруг новой оси
            //    rotation *= Quaternion.CreateFromAxisAngle(Joints[i - 1].Axis, Joints[i - 1].Value * MathF.PI / 180);
            //    Vector3 nextPoint = prevPoint + Vector3.Transform(Joints[i].StartOffset, rotation);

            //    prevPoint = nextPoint;
            //}
            ////return (prevPoint, Vector3.Transform(Vector3.Normalize(Joints.Last().StartOffset), rotation));
            var lastJoint = Tool ?? Joints.Last() as ArmPart;
            return (lastJoint.GlobalLocation, lastJoint.GlobalRotation.GetEulerAngles());
        }
    }
}
