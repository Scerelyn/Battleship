using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    public class Ship
    {
        public int Length { get; private set; }
        public string Name { get; private set; }
        public bool IsVertical { get; set; }
    }
}
