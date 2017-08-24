using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    [Serializable]
    /// <summary>
    /// Tile pieces with each state possible
    /// </summary>
    public enum TileState
    {
        Normal,Missed,Hit,ShipHere
    }
}
