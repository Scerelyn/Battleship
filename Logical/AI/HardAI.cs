using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical.AI
{
    public class HardAI : IAIModel
    {
        private Random rng = new Random();
        private string difficulty = "Hard";
        public string DifficultyTitle { get { return difficulty; } }
        private List<Point> toShoot = new List<Point>();
        public Point ChoosePoint()
        {
            throw new NotImplementedException();
        }
    }
}
