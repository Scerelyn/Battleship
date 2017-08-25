using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    [Serializable]
    public class Tile : INotifyPropertyChanged
    {
        private TileState state = TileState.Normal;
        public TileState State
        {
            get { return state; }
            set { state = value; FieldChanged(); }
        }

        [field: NonSerialized]
        /// <summary>
        /// Field changing event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void FieldChanged(string field = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(field));
            }
        }
    }
}
