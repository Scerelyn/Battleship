using BattleShip.Converters;
using BattleShip.Logical;
using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BattleShip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameData usedData = new GameData();
        TileToBrush t2bUnobf = new TileToBrush();
        TileToBrush t2bObf = new TileToBrush() { IsObfuscated = true };
        bool isPlacingShips = false;
        public MainWindow()
        {
            InitializeComponent();
            //GameData d = GameData.LoadData(Directory.GetCurrentDirectory() + "/file.dat");//new GameData();
            //d.SaveData(Directory.GetCurrentDirectory() + "/file.dat");
            //MessageBox.Show(d.PlayerShips.ValueAt(1,1)+"");
            GameData dat = new GameData();
            usedData = dat;
            FillGrids();
        }

        public void FillGrids()
        {
            for (int i = 0;  i < usedData.EnemyShips.Width; i++)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                for (int j = 0; j < usedData.EnemyShips.Height; j++)
                {
                    Rectangle r = new Rectangle()
                    {
                        Height = HitAreaStackPanel.MinHeight / usedData.EnemyShips.Height,
                        Width = HitAreaStackPanel.MinWidth / usedData.EnemyShips.Width,
                        DataContext = usedData.EnemyShips.Grid[i, j],
                        RadiusX = 5,
                        RadiusY = 5,
                    };
                    Binding b = new Binding("State");
                    b.Converter = t2bObf;
                    r.SetBinding(Rectangle.FillProperty,b);
                    sp.Children.Add(r);
                }
                HitAreaStackPanel.Children.Add(sp);
            }

            for (int i = 0; i < usedData.PlayerShips.Width; i++)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                for (int j = 0; j < usedData.PlayerShips.Height; j++)
                {
                    Rectangle r = new Rectangle()
                    {
                        Height = HitAreaStackPanel.MinHeight / usedData.PlayerShips.Height,
                        Width = HitAreaStackPanel.MinWidth / usedData.PlayerShips.Width,
                        DataContext = usedData.PlayerShips.Grid[i, j],
                        RadiusX = 5,
                        RadiusY = 5,
                    };
                    Binding b = new Binding("State");
                    b.Converter = t2bUnobf;
                    r.SetBinding(Rectangle.FillProperty, b);
                    sp.Children.Add(r);
                }
                PlayerShipAreaStackPanel.Children.Add(sp);
            }
        }

        public void PlayerPlacesShips()
        {
            List<Ship> ships = new List<Ship>()
            {
                new Ship(2,"Destroyer",false),
                new Ship(3, "Cuiser", false),
                new Ship(3, "Submarine", false),
                new Ship(4, "Battleship", false),
                new Ship(5, "Carrier", false),
            };
            isPlacingShips = true;

            foreach (Ship s in ships)
            {

            }

            isPlacingShips = false;
        }

        public void RevealEnemy()
        {
            t2bObf.IsObfuscated = false;
        }
    }
}
