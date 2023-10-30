using ARM3client.Helpers.Vectors;
using ARM3client.PathManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ARM3client.Arm
{
    public class ToolBox
    {
        public PathTool GetPathToTakeTool(KeyPoint? startPoint)
        {
            PathTool tool = new PathTool();
            tool.AddKeyPoint(new KeyPoint(5000, new Vector3(-225, 308, 44), Quaternion.CreateFromYawPitchRoll(0, 0, 82 * Vector3Helper.DegresToRadiansKoef)));
            tool.AddKeyPoint(new KeyPoint(5000, new Vector3(-247, 308, 44), Quaternion.CreateFromYawPitchRoll(0, 0, 82 * Vector3Helper.DegresToRadiansKoef)));
            tool.AddKeyPoint(new KeyPoint(5000, new Vector3(-245, 308, 85), Quaternion.CreateFromYawPitchRoll(0, 0, 82 * Vector3Helper.DegresToRadiansKoef)));
            //tool.AddKeyPoint(startPoint);
            return tool;
        }

        public PathTool GetPathToPutTool(KeyPoint? startPoint)
        {
            PathTool tool = new PathTool();
            tool.AddKeyPoint(new KeyPoint(5000, new Vector3(-245, 308, 85), Quaternion.CreateFromYawPitchRoll(0, 0, 82 * Vector3Helper.DegresToRadiansKoef)));
            tool.AddKeyPoint(new KeyPoint(5000, new Vector3(-247, 308, 44), Quaternion.CreateFromYawPitchRoll(0, 0, 82 * Vector3Helper.DegresToRadiansKoef)));
            tool.AddKeyPoint(new KeyPoint(5000, new Vector3(-225, 308, 44), Quaternion.CreateFromYawPitchRoll(0, 0, 82 * Vector3Helper.DegresToRadiansKoef)));
            //tool.AddKeyPoint(startPoint);
            return tool;
        }
    }
}
