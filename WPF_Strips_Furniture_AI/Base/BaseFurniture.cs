using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Heuristics.Actions;

namespace WPF_Strips_Furniture_AI.Base
{
    /// <summary>
    /// Basic Furniture Class
    /// </summary>
    public class BaseFurniture : ICloneable
    {
        public int ID { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }

        // [i,j]
        public int I { get; set; }
        public int J { get; set; }

        private int m_MoveCount = 0;

        public int MoveCount
        {
            get { return m_MoveCount; }
            set { m_MoveCount = value; }
        }


        // [i2,j2]
        public int I2
        {
            get
            {
                return I + Height -1;
            }
        }
        public int J2
        {
            get
            {
                return J + Width - 1;
            }
        }

        public object Clone()
        {
            return new BaseFurniture() { ID = this.ID, Height = this.Height, Width = this.Width, I = this.I, J = this.J };
        }

        /// <summary>
        /// Check if THIS and other furniture is in the same spot and has the same size
        /// </summary>
        /// <param name="f">other furniture</param>
        /// <returns>True if furnitures are the same</returns>
        public Boolean Equals(BaseFurniture f)
        {
            if (this.I == f.I &&
                this.J == f.J &&
                this.I2 == f.I2 &&
                this.J2 == f.J2)
            {
                return true;
            }
            //else
            return false;
        }


        private WrongWay m_IsWrongWay = new WrongWay() { IsInWrongWay = false };

        public WrongWay IsWrongWay
        {
            get { return m_IsWrongWay; }
            set { m_IsWrongWay = value; }
        }



    }
}
