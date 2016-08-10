using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;

namespace WPF_Strips_Furniture_AI.Heuristics
{
    public abstract class BaseAction : BaseItem
    {
        public BaseFurniture CurrentFurniture { get; set; }
        public BaseFurniture TargetFurniture { get; set; }  // the REAL LAST target furniture of the current

        /// <summary>
        /// Commit the Action
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// Check if can commit the action
        /// </summary>
        /// <returns></returns>
        public abstract Boolean CanCommit();

        /// <summary>
        /// Check where can we commit the action
        /// </summary>
        /// <returns></returns>
        public abstract List<BaseFurniture> getEmptyArea();


        public Boolean IsBlockedByWall()
        {
            var board = Model.Instance.GetCurrentBoard();   //get board

            var areaList = getEmptyArea();
            foreach (var area in areaList)
            {
                for (int i = area.I; i <= area.I2; i++)
                {
                    for (int j = area.J; j <= area.J2; j++)
                    {
                        if (board[i, j] == Consts.BOARD_WALL_SPOT)
                        {
                            return true;   //can't move because of wall
                        }
                    }
                }
            }
            return false;
        }
        //TODO (לאן לא להזיז)

    }
}
