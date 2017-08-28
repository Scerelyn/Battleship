﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical.AI
{
    public class EasyAI : IAIModel
    {
        private Random rng = new Random();
        private string difficulty = "Easy";
        public string DifficultyTitle { get { return difficulty; } }

        public Point ChoosePoint()
        {
            return new Point(rng.Next(0,Point.MaxGridX+1), rng.Next(0,Point.MaxGridY+1));
        }
    }
}
