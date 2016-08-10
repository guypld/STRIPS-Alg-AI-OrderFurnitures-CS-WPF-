using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Strips_Furniture_AI.Base
{
    /// <summary>
    /// Furniture Class (Source and Target)
    /// </summary>
    public class Furniture
    {

        BaseFurniture m_Source = new BaseFurniture();
        BaseFurniture m_Target = new BaseFurniture();

        public BaseFurniture Source
        {
            get { return m_Source; }
            set { m_Source = value; }
        }

        public BaseFurniture Target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }


    }
}
