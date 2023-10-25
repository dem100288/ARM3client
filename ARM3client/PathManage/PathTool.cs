using ARM3client.Arm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.PathManage
{
    public class PathTool
    {
        public List<KeyPoint> Path = new List<KeyPoint>();
        public float Scale = 0.2f;

        public PathTool(float scale = 0.2f)
        {
            Scale = scale;
        }

        public PathTool(KeyPoint startPoint, float scale = 0.2f) :this(scale)
        {
            Path.Add(startPoint);
        }

        public void AddKeyPoint(KeyPoint keyPoint)
        {
            Path.Add(keyPoint);
        }

        public void AddPath(PathTool path)
        {
            Path.AddRange(path.Path);
        }

        public void InterpolatePath(ArmModel armModel, bool backToStart = false)
        {
            if (Path.Count > 1)
            {
                if (backToStart)
                {
                    Path.Add(Path.First().GetCopy());
                }
                int count = Path.Count - 1;
                int ins = 1;
                for (int i = 0; i < count; i++)
                {
                    
                    KeyPoint keyPoint1 = Path[ins - 1];
                    KeyPoint keyPoint2 = Path[ins];
                    float acel = (keyPoint2.Speed - keyPoint1.Speed) / (1 / Scale);
                    float speed = keyPoint1.Speed;
                    float step = Scale;
                    while (step < 1)
                    {
                        Path.Insert(ins, 
                            new KeyPoint(
                                Convert.ToInt32(MathF.Round(speed + acel)), 
                                Vector3.Lerp(keyPoint1.Point, keyPoint2.Point, step), 
                                Quaternion.Slerp(keyPoint1.Orientation, keyPoint2.Orientation, step)
                                )
                            );
                        ins++;
                        step += Scale;
                    }
                    ins++;
                }
            }

            foreach (var point in Path)
            {
                if (armModel.CalcBackwardTask(point.Point, point.Orientation, false))
                {
                    point.JointValues = armModel.Joints.Select(x => x.TargetSteps).ToList();
                }
                else
                {
                    point.Point = Vector3.Zero;
                }
            }
            Path.RemoveAll(x => x.Point == Vector3.Zero);
        }
    }
}
