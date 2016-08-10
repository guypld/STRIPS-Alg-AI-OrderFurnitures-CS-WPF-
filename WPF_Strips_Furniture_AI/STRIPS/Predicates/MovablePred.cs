using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;

namespace WPF_Strips_Furniture_AI.Heuristics.Predicates
{
    /// <summary>
    /// Movable Predicate - indicates Movable Area
    /// </summary>
    public class MovablePred : Predicate
    {
        // reference to the movable furniture (used to know the ID)
        public BaseFurniture SourceFurniture { get; set; }

        #region EmptyAreasNeeded - list of needed empty ares, indicates by BaseFurnitures
        // presenting the destination spots to move
        // that should be empty
        private List<BaseFurniture> m_EmptyAreasNeeded;

        public List<BaseFurniture> EmptyAreasNeeded
        {
            get { return m_EmptyAreasNeeded; }
            set { m_EmptyAreasNeeded = value; }
        }
        #endregion


        // TODO (לאן אסור ללכת)


        public override Boolean isAchieved()
        {
            var board = Model.Instance.GetCurrentBoard();   //get board

            // for all empty spots furniture list
            foreach (var f in EmptyAreasNeeded)
            {
                // check if this furniture is in empty spot as needed
                for (int i = f.I; i < f.I2; i++)
                {
                    for (int j = f.J; j < f.J2; j++)
                    {
                        if (board[i, j] != Consts.BOARD_FREE_SPOT)
                        {
                            return false;   // NOT EMPTY
                        }
                    }
                }
            }

            return true;    //ok, all spots are empty, can rotate
        }

        public override string ToString()
        {
            return "MovablePred " + SourceFurniture.ID.ToString() ;
        }
    }
}
