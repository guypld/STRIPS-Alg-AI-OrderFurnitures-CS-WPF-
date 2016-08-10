using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;

namespace WPF_Strips_Furniture_AI.Heuristics
{
    /// <summary>
    /// Base Predicate - MovablePred or AtPred
    /// </summary>
    public abstract class Predicate : BaseItem
    {
        public abstract Boolean isAchieved();
    }

}
