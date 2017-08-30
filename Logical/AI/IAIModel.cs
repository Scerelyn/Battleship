using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical.AI
{
    public interface IAIModel
    {
        /// <summary>
        /// The title of the difficulty as a string for display purposes
        /// </summary>
        string DifficultyTitle { get; }
        Playgrid PlayerGrid { get; set; }
        /// <summary>
        /// The AI needs to decide on a point to fire at
        /// </summary>
        /// <returns>A Point which the AI through whatever means decides to fire at</returns>
        Point ChoosePoint();
    }
}
