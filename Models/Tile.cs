﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    [Serializable]
    public class Tile
    {
        public TileState State { get; set; } = TileState.Normal;
    }
}
