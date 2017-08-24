using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    /// <summary>
    /// Ship objects
    /// </summary>
    public class Ship
    {
        public int Length { get; private set; }
        public string Name { get; private set; }
        public bool IsVertical { get; set; }
        public Tile[] Tiles { get; set; }

        /// <summary>
        /// Determines if this ship is sunk, ie: all associated tiles are hit
        /// </summary>
        /// <returns>True if all associated tiles are hit, false if any are not</returns>
        public bool IsSunken()
        {
            foreach (Tile t in Tiles)
            {
                if (t.State != TileState.Hit)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
