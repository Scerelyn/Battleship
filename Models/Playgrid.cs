using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    [Serializable]
    /// <summary>
    /// Refers to one grid with Tiles. Player 'computer' thingys use two
    /// </summary>
    public class Playgrid : INotifyPropertyChanged
    {
        public int Height { get; private set; }
        public int Width { get; private set; }
        public Tile[,] Grid { get; private set; }

        public Playgrid(int x, int y)
        {
            Height = y;
            Width = x;
            Grid = new Tile[x, y];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i, j] = new Tile();
                }
            }
        }
        /// <summary>
        /// Changes the state of a tile
        /// </summary>
        /// <param name="x">The x coordinate of the tile to change</param>
        /// <param name="y">The y coordinate of the tile to change</param>
        /// <param name="state">The Tile state to change to</param>
        public void ChangeTile(int x, int y, TileState state)
        {
            Grid[x, y].State = state;
            FieldChanged();
        }

        /// <summary>
        /// Gets the Tile at a specified x,y coordinate
        /// Will throw an ArgumentExcption if the coordinates are out of bounds
        /// </summary>
        /// <param name="x">The x coordinate to check</param>
        /// <param name="y">The y coordinate to check</param>
        /// <returns>A tile at the given point</returns>
        public TileState ValueAt(int x, int y)
        {
            if (x >= Grid.GetLength(0) || x < 0 || y < 0 || y >= Grid.GetLength(1))
            {
                throw new ArgumentException("Cannot get a Tile value, argument out of range");
            }
            return Grid[x, y].State;
        }

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
