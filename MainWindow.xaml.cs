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
        int mouseX = 0, mouseY = 0;
        Ship activePlaceShip = null;
        public MainWindow()
        {
            InitializeComponent();
            //GameData d = GameData.LoadData(Directory.GetCurrentDirectory() + "/file.dat");//new GameData();
            //d.SaveData(Directory.GetCurrentDirectory() + "/file.dat");
            //MessageBox.Show(d.PlayerShips.ValueAt(1,1)+"");
            GameData dat = new GameData();
            usedData = dat;
            FillGrids();
            //activePlaceShip = usedData.PlayerShips[1];
            //isPlacingShips = true;
            PlayerPlacesShips();
        }

        public void FillGrids()
        {
            for (int i = 0;  i < usedData.EnemyShipsGrid.Width; i++)
            {
                StackPanel sp = new StackPanel();
                for (int j = 0; j < usedData.EnemyShipsGrid.Height; j++)
                {
                    Rectangle r = new Rectangle()
                    {
                        Height = HitAreaStackPanel.MinHeight / usedData.EnemyShipsGrid.Height,
                        Width = HitAreaStackPanel.MinWidth / usedData.EnemyShipsGrid.Width,
                        DataContext = usedData.EnemyShipsGrid.Grid[i, j],
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

            for (int i = 0; i < usedData.PlayerShipsGrid.Width; i++)
            {
                int x = i;
                StackPanel sp = new StackPanel();
                for (int j = 0; j < usedData.PlayerShipsGrid.Height; j++)
                {
                    int y = j;
                    Rectangle r = new Rectangle()
                    {
                        Height = HitAreaStackPanel.MinHeight / usedData.PlayerShipsGrid.Height,
                        Width = HitAreaStackPanel.MinWidth / usedData.PlayerShipsGrid.Width,
                        DataContext = usedData.PlayerShipsGrid.Grid[i, j],
                        RadiusX = 5,
                        RadiusY = 5,
                    };
                    Binding b = new Binding("State");
                    b.Converter = t2bUnobf;
                    r.SetBinding(Rectangle.FillProperty, b);
                    r.MouseEnter += (sender, args) =>
                    {
                        mouseX = x;
                        mouseY = y;
                        if (isPlacingShips)
                        {
                            usedData.RevertPreviewChanges(usedData.PlayerShipsGrid);
                            usedData.PreviewShipPlace(activePlaceShip,usedData.PlayerShipsGrid,mouseX,mouseY);
                        }
                        //MessageBox.Show($"{mouseX},{mouseY}");
                    };
                    r.MouseDown += DoPlaceShip;
                    sp.Children.Add(r);
                }
                PlayerShipAreaStackPanel.Children.Add(sp);
            }
        }

        public async void PlayerPlacesShips()
        {
            
            isPlacingShips = true;

            foreach (Ship s in usedData.PlayerShips)
            {
                activePlaceShip = s;
                await WhenClicked(PlayerShipAreaStackPanel);
            }

            isPlacingShips = false;
        }

        public static Task WhenClicked(StackPanel target)
        {
            var tcs = new TaskCompletionSource<object>();
            MouseButtonEventHandler onClick = null;
            onClick = (sender, e) =>
            {
                target.MouseDown -= onClick;
                tcs.TrySetResult(null);
            };
            target.MouseDown += onClick;
            return tcs.Task;
        }

        public void DoPlaceShip(object sender, RoutedEventArgs args)
        {
            MessageBox.Show($"placed {mouseX} {mouseY}");
            usedData.PlaceShip(activePlaceShip, usedData.PlayerShipsGrid, mouseX, mouseY);
        }

        public void RevealEnemy()
        {
            t2bObf.IsObfuscated = false;
        }
    }
}
