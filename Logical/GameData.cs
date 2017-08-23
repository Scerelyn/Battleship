using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical
{
    /// <summary>
    /// Stores playgrids for the player and enemy and handles operations on them
    /// </summary>
    public class GameData
    {
        /// <summary>
        /// The Player's ships, also acts as their own view of their ship, with enemy activity shown
        /// </summary>
        Playgrid PlayerShips { get; set; } = new Playgrid(10, 10);

        /// <summary>
        /// The Enemies ships, acting as their own view of the player's activity
        /// </summary>
        Playgrid EnemyShips { get; set; } = new Playgrid(10, 10);

        /// <summary>
        /// The player view of the enemy's ships
        /// Enemy ShipHere tiles will be obfuscated unless hit
        /// </summary>
        Playgrid PlayerViewEnemyShips { get; set; } = new Playgrid(10, 10);

        /// <summary>
        /// The enemy's view of the player's ships
        /// Player ShipHere tiles will be obfuscated unless hit
        /// </summary>
        Playgrid EnemyViewPlayerShips { get; set; } = new Playgrid(10, 10);

        /// <summary>
        /// Places a ship on the respective grid at the respective x,y location.
        /// Ships are placed by setting their leftmost or top most point, then add on their length
        /// </summary>
        /// <param name="s">The Ship instance to place</param>
        /// <param name="pg">The Playgrid instance to place on</param>
        /// <param name="x">The x coordinate to place on</param>
        /// <param name="y">The y coordinate to place on</param>
        public void PlaceShip(Ship s, Playgrid pg, int x, int y)
        {
            if (IsValidPlacement(s,pg,x,y))
            {
                if (s.IsVertical)
                {
                    for (int i = 0; i < pg.Height; i++)
                    {
                        pg.ChangeTile(x,y+i,Tile.ShipHere);
                    }
                }
                else
                {
                    for (int i = 0; i < pg.Width; i++)
                    {
                        pg.ChangeTile(x+i, y, Tile.ShipHere);
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if a given Ship instance can be places on a given Playgrid
        /// </summary>
        /// <param name="s">The ship instance to place</param>
        /// <param name="pg">Playgrid instance</param>
        /// <param name="x">The x coordinate to place the ship on</param>
        /// <param name="y">The y coordinate to place the ship on</param>
        /// <returns></returns>
        public bool IsValidPlacement(Ship s, Playgrid pg, int x, int y)
        {
            if (x > pg.Width-s.Length || y > pg.Height-s.Length || x < 0 || y < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Fires at a given x,y coordinate
        /// </summary>
        /// <param name="x">The x coordinate of where to 'shoot'</param>
        /// <param name="y">The y coordinate of where to 'shoot'</param>
        /// <param name="target">The target to base off of. This Playgrid will not be altered</param>
        /// <param name="affected">The playgrids to alter in state</param>
        /// <returns>True if the shot was a hit, false if it missed</returns>
        public bool Shoot(int x, int y, Playgrid target, params Playgrid[] affected)
        {
            bool hit = false;
            if (target.ValueAt(x,y) == Tile.ShipHere)
            {
                foreach(Playgrid pg in affected)
                {
                    pg.ChangeTile(x, y, Tile.Hit);
                }
                hit = true;
            }
            else
            {
                foreach (Playgrid pg in affected)
                {
                    pg.ChangeTile(x, y, Tile.Missed);
                }
            }
            return hit;
        }
    }

    
}
