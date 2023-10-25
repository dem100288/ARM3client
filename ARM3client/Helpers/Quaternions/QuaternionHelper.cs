using ARM3client.Helpers.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Helpers.Quaternions
{
    public static class QuaternionHelper
    {
        public static Vector3 GetEulerAngles(this Quaternion r)
        {

            return new Vector3(
                MathF.Asin(2.0f * (r.X * r.W - r.Y * r.Z)) * 180 / MathF.PI,
                MathF.Atan2(2.0f * (r.Y * r.W + r.X * r.Z), 1.0f - 2.0f * (r.X * r.X + r.Y * r.Y)) * 180 / MathF.PI,
                MathF.Atan2(2.0f * (r.X * r.Y + r.Z * r.W), 1.0f - 2.0f * (r.X * r.X + r.Z * r.Z)) * 180 / MathF.PI
            );
        }

        public static Quaternion GetQuaternionFromAngles(float x, float y, float z)
        {
            return Quaternion.CreateFromAxisAngle(Vector3.UnitX, x * Vector3Helper.DegresToRadiansKoef)
                * Quaternion.CreateFromAxisAngle(Vector3.UnitY, y * Vector3Helper.DegresToRadiansKoef)
                * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, z * Vector3Helper.DegresToRadiansKoef);
        }

        public static Quaternion GetQuaternionFromAngles(Vector3 vect)
        {
            return GetQuaternionFromAngles(vect.X, vect.Y, vect.Z);
        }
    }
}
