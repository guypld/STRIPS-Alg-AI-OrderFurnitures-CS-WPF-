using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Heuristics.Actions;

namespace WPF_Strips_Furniture_AI.Base
{
    public class WrongWay
    {
        private Boolean m_isInWrongWay = false;
        private DirectionEnum m_Direction;
        private int m_TryCount = 0;


        #region properties
        public Boolean IsInWrongWay
        {
            get { return m_isInWrongWay; }
            set 
            { 
                m_isInWrongWay = value;
                if (!value)
                {
                    //TryCount = 0;   //init try count
                }
            }
        }

        public DirectionEnum Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        public int TryCount
        {
            get { return m_TryCount; }
            set { m_TryCount = value; }
        }

        #endregion

    }
}
