using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;

namespace WPF_Strips_Furniture_AI.Heuristics.Predicates
{
    /// <summary>
    /// At Predicate - indicates furniture location
    /// </summary>
    public class AtPred : Predicate
    {
        #region Furniture (Source & Target)
        private Furniture m_Furniture;  //Source and Target

        public Furniture Furniture
        {
            get { return m_Furniture; }
            set { m_Furniture = value; }
        }
        #endregion

        /// <summary>
        /// is Source on Target
        /// </summary>
        public override Boolean isAchieved()
        {
            return Furniture.Source.Equals(Furniture.Target);   // if Source == Target
        }


        public override string ToString()
        {
            return Furniture.Source.ID.ToString() + " AtPred [" + Furniture.Target.I.ToString() + "," + Furniture.Target.J.ToString() + "]";
        }
    }
}
