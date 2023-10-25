using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ARM3client.Helpers.Vectors
{
    public static class Vector3Helper
    {
        public static float DegresToRadiansKoef => MathF.PI / 180;
        public static float RadiansToDegresKoef => 180 / MathF.PI;
        public static float AngleBetweenVectorsRadians(this Vector3 v1, Vector3 v2, float eps = 0.001f)
        {
            var dot = Vector3.Dot(v1, v2) / (v1.Length() * v2.Length());
            var angle_r = MathF.Acos(dot);
            if (-eps < (angle_r - MathF.PI) && (angle_r - MathF.PI) < eps)
            {
                return 0;
            }
            return angle_r;
        }

        public static float AngleBetweenVectorsDegres(this Vector3 v1, Vector3 v2)
        {
            return AngleBetweenVectorsRadians(v1, v2) * RadiansToDegresKoef;
        }

        public static Vector3 Projection(this Vector3 v, Vector3 normal, float eps = 0.001f)
        {
            var pv = v - normal * Vector3.Dot(normal, v);
            return pv.Length() < eps ? Vector3.Zero : pv;
        }

        public static Vector3 AddedLengthZ(Vector3 pointz, Vector3 pointt)
        {
            var v1 = pointz - pointt;
            var v2 = Vector3.Negate(new Vector3(pointt.X, pointt.Y, 0));
            var catet = new Vector3(pointz.X, pointz.Y, 0).Length();
            var angle = AngleBetweenVectorsRadians(v1, v2);
            var ad = catet * MathF.Tan(angle);

            if (pointt.Z > pointz.Z)
                return new Vector3(0, 0, pointz.Z - ad);
            else
                return new Vector3(0, 0, pointz.Z + ad);
        }

        public static Quaternion VectorToQuaternion(float x, float y, float z)
        {
            return Quaternion.CreateFromAxisAngle(Vector3.UnitX, x * Vector3Helper.DegresToRadiansKoef)
                * Quaternion.CreateFromAxisAngle(Vector3.UnitY, y * Vector3Helper.DegresToRadiansKoef)
                * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, z * Vector3Helper.DegresToRadiansKoef);
        }

        public static Quaternion VectorToQuaternion(Vector3 v) { return VectorToQuaternion(v.X, v.Y, v.Z); }
    }
}
