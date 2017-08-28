using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical
{
    [Serializable]
    public class Point
    {
        public static readonly int MaxGridX = 9;
        public static readonly int MaxGridY = 9;
        int x;
        int y;
        public int X
        {
            get { return x; }
            set
            {
                if (value > MaxGridX)
                {
                    throw new ArgumentException("Max Point Bounds for X exceeded");
                }
                else
                {
                    x = value;
                }
            }
        }
        public int Y
        {
            get { return y; }
            set
            {
                if (value > MaxGridY)
                {
                    throw new ArgumentException("Max Point Bounds for Y exceeded");
                }
                else
                {
                    y = value;
                }
            }
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
