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
        Brush Hit { get; set; } = Brushes.OrangeRed;
        Brush Missed { get; set; } = Brushes.White;
        Brush ShipHere { get; set; } = Brushes.LightGray;
        Brush Invalid { get; set; } = Brushes.Black;
        Brush PreviewCollision { get; set; } = Brushes.Red;
        Brush PreviewOK { get; set; } = Brushes.DimGray;
        public bool IsObfuscated { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush b = null;
            TileState t = (TileState)value;
            switch (t)
            {
                case TileState.Normal:
                    b = Normal;
                    break;
                case TileState.Missed:
                    b = Missed;
                    break;
                case TileState.Hit:
                    b = Hit;
                    break;
                case TileState.ShipHere:
                    b = IsObfuscated ? Normal : ShipHere;
                    break;
                case TileState.PreviewCollision:
                    b = PreviewCollision;
                    break;
                case TileState.PreviewOK:
                    b = PreviewOK;
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
