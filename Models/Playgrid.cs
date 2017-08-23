using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    /// <summary>
    /// Refers to one grid with Tiles. Player 'computer' thingys use two
    /// </summary>
    public class Playgrid : INotifyPropertyChanged
    {
        private Tile[,] Grid { get; set; }

        public Playgrid(int x, int y)
        {
            Grid = new Tile[x, y];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i,j] = Tile.Normal;
                }
            }
        }
        /// <summary>
        /// Changes the state of a tile
        /// </summary>
        /// <param name="x">The x coordinate of the tile to change</param>
        /// <param name="y">The y coordinate of the tile to change</param>
        /// <param name="state">The Tile state to change to</param>
        public void ChangeTile(int x, int y, Tile state)
        {
            Grid[x, y] = state;
            FieldChanged();
        }

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
