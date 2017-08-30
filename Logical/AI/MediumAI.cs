using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShip.Models;

namespace BattleShip.Logical.AI
{
    public class MediumAI : IAIModel
    {
        private Random rng = new Random();
        private string difficulty = "Medium";
        public string DifficultyTitle { get { return difficulty; } }
        public Playgrid PlayerGrid { get; set; }
        private List<Point> toShoot = new List<Point>();
        public Point ChoosePoint()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return DifficultyTitle;
        }
    }
}
