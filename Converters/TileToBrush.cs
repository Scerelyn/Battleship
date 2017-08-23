using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BattleShip.Converters
{
    public class TileToBrush : IValueConverter
    {
        Brush Normal { get; set; } = Brushes.DodgerBlue;
        Brush Hit { get; set; } = Brushes.Red;
        Brush Missed { get; set; } = Brushes.White;
        Brush ShipHere { get; set; } = Brushes.DarkGray;
        Brush Invalid { get; set; } = Brushes.Black;
        public bool IsObfuscated { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush b = null;
            Tile t = (Tile)value;
            switch (t)
            {
                case Tile.Normal:
                    b = Normal;
                    break;
                case Tile.Missed:
                    b = Missed;
                    break;
                case Tile.Hit:
                    b = Hit;
                    break;
                case Tile.ShipHere:
                    b = IsObfuscated ? Normal : ShipHere;
                    break;
                default:
                    b = Invalid;
                    break;
            }
            return b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
