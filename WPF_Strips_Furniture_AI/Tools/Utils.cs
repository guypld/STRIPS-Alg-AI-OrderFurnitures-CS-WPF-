using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;
using WPF_Strips_Furniture_AI.Heuristics.Actions;

namespace WPF_Strips_Furniture_AI.Tools
{
    public class Utils
    {
        public static int getRoomOfFurniture(BaseFurniture f)
        {
            if (f.J < Consts.DOOR_X_POS)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public static Boolean isCrossingDoor(BaseFurniture f)
        {
            if (f.J < Consts.DOOR_X_POS && (f.J + f.Width) >= Consts.DOOR_X_POS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Boolean CanCrossDoor(BaseFurniture f)
        {
            if (f.Height <= Consts.BOTTOM_DOOR_SPOT - Consts.UPPER_DOOR_SPOT + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DirectionEnum GetOppositeDirection(DirectionEnum d)
        {
            switch (d)
            {
                case DirectionEnum.Left:
                    return DirectionEnum.Right;
                case DirectionEnum.Up:
                    return DirectionEnum.Down;
                case DirectionEnum.Right:
                    return DirectionEnum.Left;
                case DirectionEnum.Down:
                default:
                    return DirectionEnum.Up;
            }
        }
    }
}
