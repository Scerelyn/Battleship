using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical.AI
{
    public class CheatingAI : IAIModel
    {
        private Random rng = new Random();
        private string difficulty = "Basically cheats";
        public string DifficultyTitle { get { return difficulty; } }
        private Playgrid Playergrid = null;

        public CheatingAI(Playgrid playergrid)
        {
            Playergrid = playergrid;
        }

        public Point ChoosePoint()
        {
            throw new NotImplementedException();
        }
    }
}
