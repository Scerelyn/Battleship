using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShip.Models;

namespace BattleShip.Logical.AI
{
    /// <summary>
    /// MediumAI is modified from HardAI, remembering only one point rather than all neighboring points on a successful hit
    /// </summary>
    public class MediumAI : IAIModel
    {
        private Random rng = new Random();
        private string difficulty = "Medium";
        public string DifficultyTitle { get { return difficulty; } }
        public Playgrid PlayerGrid { get; set; }
        private Point toShootNext = null;
        private Point lastShot = null;

        public MediumAI(Playgrid playerGrid)
        {
            PlayerGrid = playerGrid;
        }

        public Point ChoosePoint()
        {
            if (lastShot != null && PlayerGrid.ValueAt(lastShot.X, lastShot.Y) == TileState.Hit)
            {
                toShootNext = PickAdjacentPoint(lastShot.X, lastShot.Y);
            }
            if (toShootNext == null)
            {
                int x = rng.Next(0, Point.MaxGridX + 1);
                int y = rng.Next(0, Point.MaxGridY + 1);
                Point randomly = new Point(x, y); //nothing in to shoot? pick randomly
                while (PlayerGrid.ValueAt(x, y) == TileState.Missed || PlayerGrid.ValueAt(x, y) == TileState.Hit) //but dont hit a point we already hit
                {
                    x = rng.Next(0, Point.MaxGridX + 1);
                    y = rng.Next(0, Point.MaxGridY + 1);
                    randomly = new Point(x, y);
                }
                lastShot = randomly; //track it for later
                return randomly;
            }
            else
            {
                Point picked = toShootNext;
                toShootNext = null;
                return picked;
            }
        }

        public Point PickAdjacentPoint(int x, int y)
        {
            Point choosen = null;
            bool isHorizontal = rng.Next(0, 2) == 1 ? true : false; //random bool for a horizontally or vertically neighboring point
            int shift = rng.Next(0, 2) == 1 ? -1 : 1; //randomly up/left or down/right
            if (isHorizontal)
            {
                if (x + shift > PlayerGrid.Width || x + shift < 0)
                {
                    shift *= -1; //if out of bounds, flip the shift to the other direction
                }
                choosen = new Point(x + shift, y);
            }
            else
            {
                if (y + shift > PlayerGrid.Height || y + shift < 0)
                {
                    shift *= -1; //if out of bounds, flip the shift to the other direction
                }
                choosen = new Point(x, y + shift);
            }
            TileState atChoosen = PlayerGrid.ValueAt(choosen.X, choosen.Y);
            if (atChoosen == TileState.Hit || atChoosen == TileState.Missed) //avoiding the AI from getting stuck
            {
                choosen = null;
            }
            return choosen;
        }

        public override string ToString()
        {
            return DifficultyTitle;
        }
    }
}
