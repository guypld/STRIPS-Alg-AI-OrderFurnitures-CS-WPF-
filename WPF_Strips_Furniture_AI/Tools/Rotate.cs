using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;

namespace WPF_Strips_Furniture_AI.Tools
{
    public class Rotate
    {
        /// <summary>
        /// Get FULL Board (including 'f' Furniture) and check if this 'f' can be rotate)
        /// </summary>
        /// <param name="board">Full Board</param>
        /// <param name="f">Furniture to rotate</param>
        /// <returns></returns>
        public static Boolean CanRotate90Deg(int[,] board, BaseFurniture f)
        {
            if (f.Height == f.Width)    // square
            {
                return true;
            }

            //
            //  # * #         # # #
            //  # * #    -->  * * *
            //  # * #         # # #
            // 

            int maxVertex = (f.Height > f.Width) ? f.Height : f.Width;
            int halfMaxVertex = maxVertex / 2;
            int i_start = ((f.I + (f.Height / 2)) - halfMaxVertex);
            int j_start = ((f.J + (f.Width / 2)) - halfMaxVertex);

            for (int i = i_start; i < (i_start + maxVertex); i++)
            {
                for (int j = j_start; j < (j_start + maxVertex); j++)
                {

                    if ((i < 0) || (i >= 25) || (j < 0) || (j >= 13))
                    {
                        return false;
                    }

                    if ((i >= f.I) && (i <= (f.I + f.Height)) && (j >= f.J) && (j <= (f.J + f.Width)))
                    {
                        continue;
                    }
                    if (board[i, j] > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void Rotate90Deg(BaseFurniture f)
        {
            if (f.Height == f.Width)    // square
            {
                return;
            }
            //ron is the king
            // me 2
        }

    }
}
