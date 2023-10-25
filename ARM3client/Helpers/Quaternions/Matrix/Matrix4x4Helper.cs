using ARM3client.Helpers.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Helpers.Quaternions.Matrix
{
    public static class Matrix4x4Helper
    {
        public static Vector3 GetEuler(this Matrix4x4 matrix)
        {
            var angle_y = -MathF.Asin(matrix.M13);        /* Считаем ось Y */
            var D = angle_y;
            var C = MathF.Cos(angle_y);
            angle_y *= Vector3Helper.RadiansToDegresKoef;

            if (MathF.Abs(C) > 0.005f)             /* ось зафиксирована? */
            {
                var trx = matrix.M33 / C;           /* Нет, так что находим угол по X */
                var @try = -matrix.M23 / C;

                var angle_x = MathF.Atan2(@try, trx) * Vector3Helper.RadiansToDegresKoef;

                trx = matrix.M11 / C;            /* находим угол по оси Z */
                @try = -matrix.M12 / C;

                var angle_z = MathF.Atan2(@try, trx) * Vector3Helper.RadiansToDegresKoef;
                return new Vector3(float.Clamp(angle_x, -180, 180), float.Clamp(angle_y, -180, 180), float.Clamp(angle_z, -180, 180));
            }
            else                                 /* ось все-таки зафиксирована */
            {
                var angle_x = 0;                      /* Устанавливаем угол по оси X на 0 */

                var trx = matrix.M22;                 /* И считаем ось Z */
                var @try = matrix.M21;

                var angle_z = MathF.Atan2(@try, trx) * Vector3Helper.RadiansToDegresKoef;
                return new Vector3(float.Clamp(angle_x, -180, 180), float.Clamp(angle_y, -180, 180), float.Clamp(angle_z, -180, 180));
            }

        }
    }
}
