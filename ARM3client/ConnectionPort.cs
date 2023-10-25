using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using ARM3client.Commands;
using System.Diagnostics;
using ARM3client.Messages;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Input;
using System.Globalization;

namespace ARM3client
{
    public class ConnectionPort
    {
        #region static
        private static Dictionary<string, KeyValuePair<Regex, Func<string, string[], ReceivedMessage>>> MessageParsers = new();

        static ConnectionPort()
        {
            MessageParsers.Add("joint_position", new KeyValuePair<Regex, Func<string, string[], ReceivedMessage>>(new Regex(@"^j(\d+)$"), CreateJointPositionMessage));
            MessageParsers.Add("joint_find_home_end", new KeyValuePair<Regex, Func<string, string[], ReceivedMessage>>(new Regex(@"^fh(\d+)_end$"), CreateJointFindHomeMessage));
            MessageParsers.Add("find_home_end", new KeyValuePair<Regex, Func<string, string[], ReceivedMessage>>(new Regex(@"^fh_end$"), CreateFindHomeMessage));
            MessageParsers.Add("move_end", new KeyValuePair<Regex, Func<string, string[], ReceivedMessage>>(new Regex(@"^m_end$"), CreateMoveEndMessage));
            MessageParsers.Add("block_end", new KeyValuePair<Regex, Func<string, string[], ReceivedMessage>>(new Regex(@"^b_end$"), CreateBlockEndMessage));
            MessageParsers.Add("pong", new KeyValuePair<Regex, Func<string, string[], ReceivedMessage>>(new Regex(@"^PP$"), CreatePongMessage));
            MessageParsers.Add("speed_joint", new KeyValuePair<Regex, Func<string, string[], ReceivedMessage>>(new Regex(@"^speed(\d+)$"), CreateSpeedJointMessage));
            MessageParsers.Add("state_joint", new KeyValuePair<Regex, Func<string, string[], ReceivedMessage>>(new Regex(@"^st(\d+)$"), CreateStateJointMessage));
        }

        private static ReceivedMessage CreateJointPositionMessage(string num, string[] pars)
        {
            return new JointPosition(int.Parse(num), long.Parse(pars[0]));
        }

        private static ReceivedMessage CreateJointFindHomeMessage(string num, string[] pars)
        {
            return new JointFindHome(int.Parse(num));
        }

        private static ReceivedMessage CreateFindHomeMessage(string num, string[] pars)
        {
            return new FindHome(int.Parse(num));
        }

        private static ReceivedMessage CreateMoveEndMessage(string num, string[] pars)
        {
            return new MoveEnd(int.Parse(num));
        }

        private static ReceivedMessage CreateBlockEndMessage(string num, string[] pars)
        {
            return new BlockMoveEnd(int.Parse(num));
        }

        private static ReceivedMessage CreatePongMessage(string num, string[] pars)
        {
            return new PongMessage(int.Parse(num));
        }

        private static ReceivedMessage CreateSpeedJointMessage(string num, string[] pars)
        {
            return new SpeedJointMessage(int.Parse(num), double.Parse(pars[0], CultureInfo.InvariantCulture));
        }

        private static ReceivedMessage CreateStateJointMessage(string num, string[] pars)
        {
            return new JointStateMessage(int.Parse(num), pars[0] == "0" ? false : true, long.Parse(pars[1]), long.Parse(pars[2]), long.Parse(pars[3]), double.Parse(pars[4], CultureInfo.InvariantCulture));
        }

        private static ReceivedMessage ParseMessage(string message)
        {
            var pars = message.Split(' ');
            foreach (var parser in MessageParsers)
            {
                var match = parser.Value.Key.Match(pars[0]);
                if (match.Success)
                {
                    return parser.Value.Value.Invoke(match.Groups.Count > 1 ? match.Groups[1].Value : "0", pars.Skip(1).ToArray());
                }
            }
            return new UnknownMessage(message);
        }
        #endregion

        private SerialPort? _com;
        public SerialPort? Port => _com;
        string _port;
        int _baudRate;
        private string buffer = "";
        public delegate void HandleReceiveMessage(ReceivedMessage message);
        public event HandleReceiveMessage? OnReceiveMessage;
        public ObservableCollection<string> OutCommands { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> InCommands { get; set; } = new ObservableCollection<string>();
        Dispatcher? _disp;

        internal ConnectionPort(string port = "COM12", int baudRate = 115200, Dispatcher? disp = null)
        {
            _port = port;
            _baudRate = baudRate;
            _com = Reconnect();
            _disp = disp;
        }

        public SerialPort? Reconnect()
        {
            try
            {
                if (_com is not null)
                {
                    if (_com.IsOpen)
                        _com.Close();
                    _com.ErrorReceived -= Com_ErrorReceived;
                    _com.Disposed -= Com_Disposed;
                    _com.DataReceived -= Com_DataReceived;
                    _com.Dispose();
                }
                var com = new SerialPort(_port, _baudRate);
                com.ErrorReceived += Com_ErrorReceived;
                com.Disposed += Com_Disposed;
                com.DataReceived += Com_DataReceived;
                com.NewLine = "\n";
                com.Open();
                return com;
            } 
            catch (Exception ex)
            { 
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            buffer += _com?.ReadExisting() ?? "";
            buffer = buffer.Replace("\0", "");
            var l = buffer.Length;
            if (buffer.Contains('\n'))
            {
                var mess = buffer.Split("\n");
                foreach (var mes in mess.SkipLast(1))
                {
                    _disp?.Invoke(() => InCommands.Add(mes));
                    var message = ParseMessage(mes);
                    if (message is not UnknownMessage)
                    {
                        OnReceiveMessage?.Invoke(message);
                    }
                }
                buffer = mess.Last();
            }
        }

        private void Com_Disposed(object? sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Com_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        internal void SendCommand(ArmCommand command)
        {
            if (_com is not null && _com.IsOpen)
            {
                _disp?.Invoke(() => OutCommands.Add(command.ToStringCommand()));
                _com.WriteLine(command.ToStringCommand());
            }
        }
    }
}
