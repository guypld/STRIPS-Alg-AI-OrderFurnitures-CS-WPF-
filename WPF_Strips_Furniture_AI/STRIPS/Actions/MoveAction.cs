using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;

namespace WPF_Strips_Furniture_AI.Heuristics.Actions
{
    public enum DirectionEnum 
    {
        Left,
        Up,
        Right,
        Down
    }

    public class MoveAction : BaseAction
    {
        public DirectionEnum Direction { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public override void Commit()
        {
            x = CurrentFurniture.I;
            y = CurrentFurniture.J;

            CurrentFurniture.I = CurrentFurniture.I + GetIDirection();
            CurrentFurniture.J = CurrentFurniture.J + GetJDirection();

            CurrentFurniture.MoveCount++;
        }

        /// <summary>
        /// Check if this furniture can be moved to specific direction
        /// </summary>
        /// <returns></returns>
        public override bool CanCommit()
        {
            var board = Model.Instance.GetCurrentBoard();   //get board

            // get empty space needed by BaseFurniture
            // in MoveAction, there is always only 1 BaseFurniture
            var f = getEmptyArea().First(); 

            for (int i = f.I ; i <= f.I2 ; i++)
            {
                for (int j = f.J ; j <= f.J2 ; j++)
                {
                    if (board[i, j] != Consts.BOARD_FREE_SPOT)
                    {
                        return false;   //can't move
                    }
                }
            }

            return true;    // can move!
        }

        public override List<BaseFurniture> getEmptyArea()
        {
            BaseFurniture newEmptyFur = new BaseFurniture();

            if (Direction == DirectionEnum.Up)
            {
                newEmptyFur.I = CurrentFurniture.I - 1;
                newEmptyFur.Height = 1;
                newEmptyFur.J = CurrentFurniture.J;
                newEmptyFur.Width = CurrentFurniture.Width;
            }
            else if (Direction == DirectionEnum.Down)
            {
                newEmptyFur.I = CurrentFurniture.I + CurrentFurniture.Height;
                newEmptyFur.Height = 1;
                newEmptyFur.J = CurrentFurniture.J;
                newEmptyFur.Width = CurrentFurniture.Width;
            }
            else if (Direction == DirectionEnum.Left)
            {
                newEmptyFur.J = CurrentFurniture.J - 1;
                newEmptyFur.Width = 1;
                newEmptyFur.I = CurrentFurniture.I;
                newEmptyFur.Height = CurrentFurniture.Height;

            }
            else if (Direction == DirectionEnum.Right)
            {
                newEmptyFur.J = CurrentFurniture.J + CurrentFurniture.Width;
                newEmptyFur.Width = 1;
                newEmptyFur.I = CurrentFurniture.I;
                newEmptyFur.Height = CurrentFurniture.Height;
            }

            return new List<BaseFurniture>() { newEmptyFur };
        }

        /// <summary>
        /// +1 \ -1 \ 0  (Horizontal)
        /// </summary>
        private int GetJDirection()
        {
            switch (Direction)
            {
                case DirectionEnum.Left:
                    return -1;
                case DirectionEnum.Up:
                    return 0;
                case DirectionEnum.Right:
                    return 1;
                case DirectionEnum.Down:
                    return 0;
                default:
                    return 0; // never
            }
        }

        /// <summary>
        /// +1 \ -1 \ 0  (Vertical)
        /// </summary>
        private int GetIDirection()
        {
            switch (Direction)
            {
                case DirectionEnum.Left:
                    return 0;
                case DirectionEnum.Up:
                    return -1;
                case DirectionEnum.Right:
                    return 0;
                case DirectionEnum.Down:
                    return 1;
                default:
                    return 0; // never
            }
        }

        public override string ToString() 
        {
            return "Move " + Direction.ToString();
        }


    }
}
