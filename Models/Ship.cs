using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    [Serializable]
    /// <summary>
    /// Ship objects
    /// </summary>
    public class Ship
    {
        public int Length { get; private set; }
        public string Name { get; private set; }
        public bool IsVertical { get; set; }
        public Tile[] Tiles { get; set; } = null;
        /// <summary>
        /// Tells if the ship was recently sank. 
        /// Null means the ship is still up, 
        /// false means the ship has already sunken for more than one turn, 
        /// true means the ship just sank one turn ago
        /// </summary>
        public bool? JustSank { get; private set; } = null; 
        public Ship(int length, string name, bool isVertical)
        {
            Length = length;
            Name = name;
            IsVertical = isVertical;
        }

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
            //if the for loop does not hit the return, all tiles are hit and the ship is sunken
            if (JustSank == null) //if its null, the ship just sank
            {
                JustSank = true; 
            }
            else if (JustSank ?? false) //if its true, the ship has sank for more than one turn
            {
                JustSank = false;
            }
            return true;
        }
    }
}
