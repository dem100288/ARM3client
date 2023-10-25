using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using ARM3client.Arm;
using ARM3client.Commands;
using ARM3client.Helpers.Quaternions;
using ARM3client.PathManage;

using Amazon.SQS;
using Amazon.SQS.Model;
using System.Timers;
using Amazon.Runtime;
using ARM3client.Helpers.Vectors;

namespace ARM3client
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ArmModel armModel;
        static string queueUrl = "https://message-queue.api.cloud.yandex.net/b1gh8sdg8g76j84nk25r/dj600000000lvpff04hi/robo_arm.fifo";
        static List<string> attributeNames = new List<string>() { "All" };
        private string _accessKey = "YCAJEMCQiSx9_-SHUUi1VnWdL";
        private string _secretKey = "YCMH4q2liyAEzjmjBnhjaNYMVswhJNA3kocDMy_2";
        AmazonSQSClient client;
        PathTool wave_hand = new PathTool();
        ReceiveMessageRequest request = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            AttributeNames = attributeNames,
            
        };

        Timer timer = new Timer(2000);

        public MainWindow()
        {
            InitializeComponent();

            _accessKey.Contains("помаш");

            AmazonSQSConfig configsSQS = new AmazonSQSConfig
            {
                ServiceURL = "https://message-queue.api.cloud.yandex.net",
                AuthenticationRegion = "ru-central1",
            };
            BasicAWSCredentials credentials = new BasicAWSCredentials(_accessKey, _secretKey);
            client = new AmazonSQSClient(credentials, configsSQS);
            
            armModel = ArmModel.CreateAR3AnninRobotics(Dispatcher);
            foreach (var joint in armModel.Joints)
            {
                JointsPanel.Children.Add(new JointControl(joint, armModel));
            }
            wave_hand.AddPath(armModel.ToolBox.GetPathToTakeTool(null));
            wave_hand.AddKeyPoint(new KeyPoint(5000, new Vector3(0, 323, 474), Quaternion.Identity));
            wave_hand.AddKeyPoint(new KeyPoint(5000, new Vector3(0, 300, 450), Quaternion.CreateFromYawPitchRoll(0, 75 * Vector3Helper.DegresToRadiansKoef, 0)));
            wave_hand.AddKeyPoint(new KeyPoint(8000, new Vector3(30, 300, 450), Quaternion.CreateFromYawPitchRoll(30 * Vector3Helper.DegresToRadiansKoef, 75 * Vector3Helper.DegresToRadiansKoef, 0)));
            wave_hand.AddKeyPoint(new KeyPoint(8000, new Vector3(0, 300, 450), Quaternion.CreateFromYawPitchRoll(0, 75 * Vector3Helper.DegresToRadiansKoef, 0)));
            wave_hand.AddKeyPoint(new KeyPoint(8000, new Vector3(-30, 300, 450), Quaternion.CreateFromYawPitchRoll(-30 * Vector3Helper.DegresToRadiansKoef, 75 * Vector3Helper.DegresToRadiansKoef, 0)));
            wave_hand.AddKeyPoint(new KeyPoint(8000, new Vector3(0, 300, 450), Quaternion.CreateFromYawPitchRoll(0, 75 * Vector3Helper.DegresToRadiansKoef, 0)));
            wave_hand.AddKeyPoint(new KeyPoint(8000, new Vector3(30, 300, 450), Quaternion.CreateFromYawPitchRoll(30 * Vector3Helper.DegresToRadiansKoef, 75 * Vector3Helper.DegresToRadiansKoef, 0)));
            wave_hand.AddKeyPoint(new KeyPoint(8000, new Vector3(0, 300, 450), Quaternion.CreateFromYawPitchRoll(0, 75 * Vector3Helper.DegresToRadiansKoef, 0)));
            wave_hand.AddKeyPoint(new KeyPoint(5000, new Vector3(0, 323, 474), Quaternion.Identity));
            wave_hand.AddPath(armModel.ToolBox.GetPathToPutTool(null));
            wave_hand.AddKeyPoint(new KeyPoint(5000, new Vector3(0, 323, 474), Quaternion.Identity));
            wave_hand.InterpolatePath(armModel);

            listBoxIn.ItemsSource = armModel.InCom;
            listBoxOut.ItemsSource = armModel.OutCom;
            DataContext = armModel;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var response = await client.ReceiveMessageAsync(request);

            if (response.Messages.Count > 0)
            {
                foreach (var message in response.Messages)
                {
                    if (message.Body == "wave_hand")
                    {
                        if (armModel.ArmIsInit)
                        {
                            armModel.MovePath(wave_hand);
                        }
                        await client.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                    }
                }
                
            }
        }

        private void FindHome_Click(object sender, RoutedEventArgs e)
        {
            armModel.ArmInit();
        }

        private void Ping_Click(object sender, RoutedEventArgs e)
        {
            armModel.Ping();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            armModel.MoveToZero();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //var p = new PathTool();
            //p.AddKeyPoint(new KeyPoint(500, new Vector3(-100, 400, 400), Quaternion.Identity));
            //p.AddKeyPoint(new KeyPoint(1000, new Vector3(100, 400, 400), Quaternion.Identity));
            //p.AddKeyPoint(new KeyPoint(500, new Vector3(100, 400, 200), Quaternion.Identity));
            //p.AddKeyPoint(new KeyPoint(1000, new Vector3(-100, 400, 200), Quaternion.Identity));
            //p.AddKeyPoint(new KeyPoint(500, new Vector3(-100, 400, 400), Quaternion.Identity));
            //p.InterpolatePath(armModel);

            armModel.MovePath(wave_hand);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //armModel.CalcBackwardTask(new Vector3(-50, 323.08f, 474.77f), new Vector3(0f, 0f, 0f));
            //var p = new PathTool();
            //p.AddKeyPoint(new KeyPoint(500, new Vector3(-100, 400, 400), QuaternionHelper.GetQuaternionFromAngles(90, -45, 0)));
            //p.AddKeyPoint(new KeyPoint(1000, new Vector3(100, 400, 400), QuaternionHelper.GetQuaternionFromAngles(90, 45, 0)));
            //p.AddKeyPoint(new KeyPoint(500, new Vector3(100, 400, 200), QuaternionHelper.GetQuaternionFromAngles(-90, -45, 0)));
            //p.AddKeyPoint(new KeyPoint(1000, new Vector3(-100, 400, 200), QuaternionHelper.GetQuaternionFromAngles(-90, 45, 0)));
            //p.AddKeyPoint(new KeyPoint(500, new Vector3(-100, 400, 400), QuaternionHelper.GetQuaternionFromAngles(90, -45, 0)));
            //p.InterpolatePath(armModel);

            var p = new PathTool();
            p.AddPath(armModel.ToolBox.GetPathToPutTool(null));
            p.AddKeyPoint(new KeyPoint(5000, new Vector3(0, 323, 474), Quaternion.Identity));
            p.InterpolatePath(armModel);

            armModel.MovePath(p);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            var wp = new ControlPoint(armModel);
            wp.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
        }
    }
}
