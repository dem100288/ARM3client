using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Commands
{
    /// <summary>
    /// Перемещение к новому положению всех осей с максимальной скоростью
    /// </summary>
    public class MoveToPositionMaxSpeedCommand : ArmCommand
    {
        public MoveToPositionMaxSpeedCommand(long joint1, long joint2, long joint3, long joint4, long joint5, long joint6)
        {
            Joint1Position = joint1;
            Joint2Position = joint2;
            Joint3Position = joint3;
            Joint4Position = joint4;
            Joint5Position = joint5;
            Joint6Position = joint6;
            Name = "MS";
        }
        public long Joint1Position { get; set; }
        public long Joint2Position { get; set; }
        public long Joint3Position { get; set; }
        public long Joint4Position { get; set; }
        public long Joint5Position { get; set; }
        public long Joint6Position { get; set; }
        public override string ToStringCommand()
        {
            return $"{Name} {Joint1Position.ToString().PadLeft(5, '0')} {Joint2Position.ToString().PadLeft(5, '0')} {Joint3Position.ToString().PadLeft(5, '0')} {Joint4Position.ToString().PadLeft(5, '0')} {Joint5Position.ToString().PadLeft(5, '0')} {Joint6Position.ToString().PadLeft(5, '0')}";
        }
    }
}
