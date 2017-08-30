using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical.AI
{
    [Serializable]
    /// <summary>
    /// Hard AI will randomly fire until a shot hits, in which case all surrounding points are added to a 'shot queue' to fire on next. When that queue empties, shots are fired randomly again
    /// </summary>
    public class HardAI : IAIModel
    {
        private Random rng = new Random();
        private string difficulty = "Hard";
        public string DifficultyTitle { get { return difficulty; } }
        private List<Point> toShoot = new List<Point>();
        private Playgrid playerGrid;
        private Point lastShot = null;

        public HardAI(Playgrid playerGrid)
        {
            this.playerGrid = playerGrid;
        }

        public Point ChoosePoint()
        {
            CheckAndClearList();
            if(lastShot != null && playerGrid.ValueAt(lastShot.X, lastShot.Y) == TileState.Hit) //checking to see if the last shot was a hit
            {
                AddValidSurrounding(lastShot.X, lastShot.Y); //if so, add all surounding points as possible ship locations
            }
            if (toShoot.Any()) //if any points remain in the toShoot list,
            {
                Point toHit = toShoot[0]; //pick the first
                toShoot.RemoveAt(0); //and remove it
                lastShot = toHit; //track later
                return toHit;
            }
            else
            {
                int x = rng.Next(0, Point.MaxGridX + 1);
                int y = rng.Next(0, Point.MaxGridY + 1);
                Point randomly = new Point(x, y); //nothing in to shoot? pick randomly
                while (playerGrid.ValueAt(x,y) == TileState.Missed || playerGrid.ValueAt(x,y) == TileState.Hit) //but dont hit a point we already hit
                {
                    x = rng.Next(0, Point.MaxGridX + 1);
                    y = rng.Next(0, Point.MaxGridY + 1);
                    randomly = new Point(x, y);
                }
                lastShot = randomly; //track it for later
                return randomly;
            }
        }

        /// <summary>
        /// Adds all surrounding points relative to the given x,y coordinates to the toShoot list unless they are out of bounds or already fired at
        /// </summary>
        /// <param name="x">The x coordinate of the point to check around</param>
        /// <param name="y">The y coordinate of the point to check around</param>
        private void AddValidSurrounding(int x, int y)
        {
            if (x - 1 >= 0 && (playerGrid.ValueAt(x - 1, y) != TileState.Missed && playerGrid.ValueAt(x - 1, y) != TileState.Hit))
            {
                toShoot.Add(new Point(x - 1,y));
            }
            if (x + 1 <= Point.MaxGridX && (playerGrid.ValueAt(x + 1, y) != TileState.Missed && playerGrid.ValueAt(x + 1, y) != TileState.Hit))
            {
                toShoot.Add(new Point(x + 1, y));
            }
            if (y - 1 >= 0 && (playerGrid.ValueAt(x, y - 1) != TileState.Missed && playerGrid.ValueAt(x, y - 1) != TileState.Hit))
            {
                toShoot.Add(new Point(x, y - 1));
            }
            if (y + 1 <= Point.MaxGridY && (playerGrid.ValueAt(x, y + 1) != TileState.Missed && playerGrid.ValueAt(x, y + 1) != TileState.Hit))
            {
                toShoot.Add(new Point(x, y + 1));
            }
        }

        /// <summary>
        /// Checks the toShoot list and removes any points that have been already fired at to avoid overlapping points being added and hence wasted turns
        /// </summary>
        public void CheckAndClearList()
        {
            List<Point> newToShoot = new List<Point>();
            foreach (Point p in toShoot)
            {
                TileState valAtP = playerGrid.ValueAt(p.X, p.Y);
                if (valAtP != TileState.Missed && valAtP != TileState.Hit) 
                {
                    //doing a .remove(p) on toShoot may not have expected behavior on the loop, so add to a new list and replace old list instance would be safer
                    newToShoot.Add(p);
                }
            }
            toShoot = newToShoot;
        }

        public override string ToString()
        {
            return DifficultyTitle;
        }
    }
}
