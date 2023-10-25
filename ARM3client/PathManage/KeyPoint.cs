using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.PathManage
{
    public class KeyPoint
    {
        public int Speed { get; set; }
        public Vector3 Point { get; set; }
        public Quaternion Orientation { get; set; }
        public List<long> JointValues { get; set; } = new List<long>();

        public KeyPoint(int speed, Vector3 point, Quaternion orientation)
        {
            Speed = speed;
            Point = point;
            Orientation = orientation;
        }

        public KeyPoint GetCopy()
        {
            return new KeyPoint(Speed, Point, Orientation);
        }
    }
}
