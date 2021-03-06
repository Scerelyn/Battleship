﻿using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical.AI
{
    [Serializable]
    /// <summary>
    /// Easy AI, exclusively picks points at random
    /// </summary>
    public class EasyAI : IAIModel
    {
        private Random rng = new Random();
        private string difficulty = "Easy";
        public string DifficultyTitle { get { return difficulty; } }
        public Playgrid PlayerGrid { get; set; }

        public EasyAI(Playgrid playerGrid)
        {
            this.PlayerGrid = playerGrid;
        }

        public Point ChoosePoint()
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
            return randomly;
        }

        public override string ToString()
        {
            return DifficultyTitle;
        }
    }
}
