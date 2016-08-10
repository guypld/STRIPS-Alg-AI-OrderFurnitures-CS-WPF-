using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;
using WPF_Strips_Furniture_AI.Tools;

namespace WPF_Strips_Furniture_AI
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private BaseFurniture m_newFurniture = new BaseFurniture() { Height = 2, Width = 2 };   // Temp Furniture, When adding new from GUI
        private Boolean m_CanPutNewFurnitureState = true;
        private ObservableCollection<ActionDescription> m_Moves = new ObservableCollection<ActionDescription>();


        public BaseFurniture NewFurniture
        {
            get { return m_newFurniture; }
            set 
            { 
                m_newFurniture = value;
                OnPropertyChanged("NewFurniture");
            }
        }

        public Boolean CanPutNewFurnitureState
        {
            get { return m_CanPutNewFurnitureState; }
            set 
            {
                m_CanPutNewFurnitureState = value;
                OnPropertyChanged("CanPutNewFurnitureState");
            }
        }

        public ObservableCollection<ActionDescription> Moves
        {
            get { return m_Moves; }
            set { m_Moves = value; }
        }


        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
