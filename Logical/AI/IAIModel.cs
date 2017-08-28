using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical.AI
{
    public interface IAIModel
    {
        string DifficultyTitle { get; }
        Point ChoosePoint();
    }
}
