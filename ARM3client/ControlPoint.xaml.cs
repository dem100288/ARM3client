using ARM3client.Arm;
using ARM3client.Helpers.Quaternions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _3DconnexionDriver;
using System.Timers;
using ARM3client.Helpers.Vectors;

namespace ARM3client
{
    /// <summary>
    /// Логика взаимодействия для ControlPoint.xaml
    /// </summary>
    public partial class ControlPoint : Window
    {
        private ArmModel _model;
        private Vector3 _position;
        private Quaternion _rotation;
        _3DconnexionDevice? dev;
        private float tx, ty, tz, rx, ry, rz;
        private static float max_axis = 2500;
        private static float speed = 0.01f;
        private float divider = max_axis / (MathF.PI / 2);
        public ControlPoint(ArmModel model)
        {
            _model = model;
            _position = _model.PositionEndPoint;
            _rotation = _model.TargetRotation == Quaternion.Zero ? Quaternion.Identity : _model.TargetRotation;
            tx = ty = tz = rx = ry = rz = 0;
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _position += new Vector3(2, 0, 0);
            _model.CalcBackwardTask(_position, _rotation);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _position += new Vector3(-2, 0, 0);
            _model.CalcBackwardTask(_position, _rotation);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            _position += new Vector3(0, 2, 0);
            _model.CalcBackwardTask(_position, _rotation);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            _position += new Vector3(0, -2, 0);
            _model.CalcBackwardTask(_position, _rotation);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            _position += new Vector3(0, 0, 2);
            _model.CalcBackwardTask(_position, _rotation);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            _position += new Vector3(0, 0, -2);
            _model.CalcBackwardTask(_position, _rotation);
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            _rotation = QuaternionHelper.GetQuaternionFromAngles(2, 0, 0) * _rotation;
            _model.CalcBackwardTask(_model.PositionEndPoint, _rotation);
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            _rotation = QuaternionHelper.GetQuaternionFromAngles(-2, 0, 0) * _rotation;
            _model.CalcBackwardTask(_model.PositionEndPoint, _rotation);
        }

        private void button3_Copy_Click(object sender, RoutedEventArgs e)
        {
            _rotation = QuaternionHelper.GetQuaternionFromAngles(0, 2, 0) * _rotation;
            _model.CalcBackwardTask(_model.PositionEndPoint, _rotation);
        }

        private void button2_Copy_Click(object sender, RoutedEventArgs e)
        {
            _rotation = QuaternionHelper.GetQuaternionFromAngles(0, -2, 0) * _rotation;
            _model.CalcBackwardTask(_model.PositionEndPoint, _rotation);
        }

        private void button5_Copy_Click(object sender, RoutedEventArgs e)
        {
            _rotation = QuaternionHelper.GetQuaternionFromAngles(0, 0, 2) * _rotation;
            _model.CalcBackwardTask(_model.PositionEndPoint, _rotation);
        }

        private void button4_Copy_Click(object sender, RoutedEventArgs e)
        {
            _rotation = QuaternionHelper.GetQuaternionFromAngles(0, 0, -2) * _rotation;
            _model.CalcBackwardTask(_model.PositionEndPoint, _rotation);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dev.CloseDevice();
            dev.Dispose();
            dev = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var _handle = new HandleRef(this, new WindowInteropHelper(this).Handle);
                dev = new _3DconnexionDevice("AR3 arm");
                dev.Motion += Dev_Motion;
                dev.ZeroPoint += Dev_ZeroPoint;
                dev.InitDevice((IntPtr)_handle);
            }
            catch (Exception ex)
            { }
        }

        private void Dev_Motion(object? sender, MotionEventArgs e)
        {

            tx += (MathF.Tan((e.TX / divider)) * Vector3Helper.RadiansToDegresKoef) * speed;
            ty += (MathF.Tan((e.TY / divider)) * Vector3Helper.RadiansToDegresKoef) * speed;
            tz += (MathF.Tan((e.TZ / divider)) * Vector3Helper.RadiansToDegresKoef) * speed;
            rx += (MathF.Tan((e.RX / divider)) * Vector3Helper.RadiansToDegresKoef) * speed;
            ry += (MathF.Tan((e.RY / divider)) * Vector3Helper.RadiansToDegresKoef) * speed;
            rz += (MathF.Tan((e.RZ / divider)) * Vector3Helper.RadiansToDegresKoef) * speed;

            //bool f = false;
            _position += new Vector3(tx, tz, ty);
            _rotation = QuaternionHelper.GetQuaternionFromAngles(rx, rz, ry) * _rotation;
            //if (tx > 1f || tx < -1f)
            //{
            //    _position += new Vector3(tx, 0, 0);
            //    tx = 0;
            //    f = true;
            //}

            //if (tz > 1f || tz < -1f)
            //{
            //    _position += new Vector3(0, tz, 0);
            //    tz = 0;
            //    f = true;
            //}

            //if (ty > 1f || ty < -1f)
            //{
            //    _position += new Vector3(0, 0, ty);
            //    ty = 0;
            //    f = true;
            //}

            //if (rx > 1f || rx < -1f)
            //{
            //    _rotation = QuaternionHelper.GetQuaternionFromAngles(rx, 0, 0) * _rotation;
            //    rx = 0;
            //    f = true;
            //}

            //if (ry > 1f || ry < -1f)
            //{
            //    _rotation = QuaternionHelper.GetQuaternionFromAngles(0, 0, ry) * _rotation;
            //    ry = 0;
            //    f = true;
            //}

            //if (rz > 1f || rz < -1f)
            //{
            //    _rotation = QuaternionHelper.GetQuaternionFromAngles(0, rz, 0) * _rotation;
            //    rz = 0;
            //    f = true;
            //}

            _model.CalcBackwardTask(_position, _rotation);
            //Debug.WriteLine($"{e.TX} {e.TY} {e.TZ} {e.RX} {e.RY} {e.RZ}");
        }

        private void Dev_ZeroPoint(object? sender, EventArgs e)
        {
            tx = 0;
            ty = 0;
            tz = 0;
            rx = 0;
            ry = 0;
            rz = 0;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (dev is not null && dev.IsAvailable)
            {
                dev.ProcessWindowMessage(msg, wParam, lParam);
            }
            return IntPtr.Zero;
        }
    }
}
