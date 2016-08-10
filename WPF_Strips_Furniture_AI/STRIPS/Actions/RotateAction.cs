using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;

namespace WPF_Strips_Furniture_AI.Heuristics.Actions
{
    public class RotateAction : BaseAction
    {
        public override void Commit()
        {
            CurrentFurniture.MoveCount++;

            if (this.CurrentFurniture.Height == this.CurrentFurniture.Width)    // square
            {
                return;
            }

            int maxVertex = (CurrentFurniture.Height > CurrentFurniture.Width) ? CurrentFurniture.Height : CurrentFurniture.Width;
            int halfMaxVertex = maxVertex / 2;
            int cornerI = ((CurrentFurniture.I + (CurrentFurniture.Height / 2)) - halfMaxVertex);
            int cornerJ = ((CurrentFurniture.J + (CurrentFurniture.Width / 2)) - halfMaxVertex);

            //Horizontal
            if (CurrentFurniture.Height < CurrentFurniture.Width)
            {
                CurrentFurniture.J = cornerJ + (CurrentFurniture.I - cornerI);
                CurrentFurniture.I = cornerI;
            }
                //Vertical
            else
            {
                CurrentFurniture.I = cornerI + (CurrentFurniture.J - cornerJ);
                CurrentFurniture.J = cornerJ;
            }
            

            int tmpWidth = CurrentFurniture.Width;
            CurrentFurniture.Width = CurrentFurniture.Height;
            CurrentFurniture.Height = tmpWidth;
        }

        public override bool CanCommit()
        {
            if (this.CurrentFurniture.Height == this.CurrentFurniture.Width)    // square
            {
                return true;
            }
            
            var board = Model.Instance.GetCurrentBoard();

            int maxVertex = (CurrentFurniture.Height > CurrentFurniture.Width) ? CurrentFurniture.Height : CurrentFurniture.Width;
            int halfMaxVertex = maxVertex / 2;
            int i_start = ((CurrentFurniture.I + (CurrentFurniture.Height / 2)) - halfMaxVertex);
            int j_start = ((CurrentFurniture.J + (CurrentFurniture.Width / 2)) - halfMaxVertex);

            for (int i = i_start; i < (i_start + maxVertex); i++)
            {
                for (int j = j_start; j < (j_start + maxVertex); j++)
                {
                    if ((i < 0) || (i >= board.GetLength(0)) || (j < 0) || (j >= board.GetLength(1)))
                    {
                        return false;
                    }

                    if ((i >= CurrentFurniture.I) && 
                        (i <= (CurrentFurniture.I + CurrentFurniture.Height - 1)) && 
                        (j >= CurrentFurniture.J) && 
                        (j <= (CurrentFurniture.J + CurrentFurniture.Width - 1)))
                    {
                        continue;
                    }
                    if (board[i, j] > 0 || board[i, j] == Consts.BOARD_WALL_SPOT)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override List<BaseFurniture> getEmptyArea()
        {
            List<BaseFurniture> list = new List<BaseFurniture>();

            var board = Model.Instance.GetCurrentBoard();

            int maxVertex = (CurrentFurniture.Height > CurrentFurniture.Width) ? CurrentFurniture.Height : CurrentFurniture.Width;
            int halfMaxVertex = maxVertex / 2;
            int i_start = ((CurrentFurniture.I + (CurrentFurniture.Height / 2)) - halfMaxVertex);
            int j_start = ((CurrentFurniture.J + (CurrentFurniture.Width / 2)) - halfMaxVertex);

            BaseFurniture f1 = new BaseFurniture();
            BaseFurniture f2 = new BaseFurniture();


            //Horizontal
            if (CurrentFurniture.Height < CurrentFurniture.Width)
            {
                f1.I = i_start;
                f1.J = j_start;
                f1.Width = CurrentFurniture.Width;
                f1.Height = CurrentFurniture.I - i_start;

                f2.I = CurrentFurniture.I2 + 1;
                f2.J = j_start;
                f2.Width = CurrentFurniture.Width;
                f2.Height = CurrentFurniture.Width - (f1.Height + CurrentFurniture.Height);
            }
            else
            {
                f1.I = i_start;
                f1.J = j_start;
                //f1.Width = i_start - CurrentFurniture.I;
                f1.Width = CurrentFurniture.J - j_start;
                f1.Height = CurrentFurniture.Height;

                f2.I = CurrentFurniture.I;
                f2.J = CurrentFurniture.J2 + 1;
                f2.Width = CurrentFurniture.Height - (f1.Width + CurrentFurniture.Width);
                f2.Height = CurrentFurniture.Height;
            }
            list.Add(f1);
            list.Add(f2);

            return list;
        }


        public override string ToString()
        {
            return "Rotate";
        }
    }
}
