using ARM3client.Arm;
using ARM3client.Arm.Joints;
using ARM3client.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ARM3client
{
    /// <summary>
    /// Логика взаимодействия для JointControl.xaml
    /// </summary>
    public partial class JointControl : UserControl
    {
        Joint _joint;
        ArmModel _model;
        public JointControl(Joint joint, ArmModel model)
        {
            InitializeComponent();

            _joint = joint;
            _model = model;
            DataContext = _joint;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _model._port.SendCommand(new FindHomeCommand(_joint.NumJoint, true));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _model._port.SendCommand(new TestLimmitJoint(_joint.NumJoint));
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_joint.isInit)
            {
                _model.ManualMove = true;
                _joint.SetTargetValue(Convert.ToSingle(valueSlider.Value));
            }
        }
    }
}
