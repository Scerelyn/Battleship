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
    /// CheatingAI will simply look for ships directly and fire there, as you'd expect from a cheater
    /// </summary>
    public class CheatingAI : IAIModel
    {
        private Random rng = new Random();
        private string difficulty = "Basically cheats, for debugging win/lose states or you're suicidal";
        public string DifficultyTitle { get { return difficulty; } }
        public Playgrid PlayerGrid { get; set; }
        private Playgrid Playergrid = null;

        public CheatingAI(Playgrid playergrid)
        {
            Playergrid = playergrid;
        }

        public Point ChoosePoint()
        {
            Point choosen = null;
            for (int x = 0; x < PlayerGrid.Width; x++)
            {
                for (int y = 0; y < PlayerGrid.Height; y++)
                {
                    if (PlayerGrid.ValueAt(x,y) == TileState.ShipHere) //just scan and see if a ship is here
                    {
                        choosen = new Point(x,y); //then shoot at it
                    }
                }
            }
            return choosen;
        }

        public override string ToString()
        {
            return DifficultyTitle;
        }
    }
}
