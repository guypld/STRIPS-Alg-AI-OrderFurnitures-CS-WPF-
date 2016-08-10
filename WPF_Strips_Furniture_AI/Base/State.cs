using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;

namespace WPF_Strips_Furniture_AI
{
    public class State
    {
        private List<BaseFurniture> m_FurnitureList;

        public List<BaseFurniture> FurnitureList
        {
            get { return m_FurnitureList; }
            set { m_FurnitureList = value; }
        }


        public State()
        {
            FurnitureList = new List<BaseFurniture>();
        }

        // clone Range
        public State(List<BaseFurniture> list)
        {
            FurnitureList = new List<BaseFurniture>();
            // clone exist furniture list
            foreach (var item in list)
            {
                FurnitureList.Add(item.Clone() as BaseFurniture);
            }
        }
        
        /// <summary>
        /// Get Board Coppy with State Furnitures
        /// </summary>
        public int[,] GetBoard()
        {
            int[,] tmpBoard = Model.CloneNewBoard();

            // for all furnitures
            foreach (var f in FurnitureList)
            {
                // put on board
                for (int i = f.I; i < (f.I + f.Height); i++)
                {
                    for (int j = f.J; j < (f.J + f.Width); j++)
                    {
                        tmpBoard[i, j] = f.ID;
                    }
                }
            }

            return tmpBoard;
        }
    }
}
