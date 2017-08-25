﻿using BattleShip.Converters;
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
            try
            {
                PlayerPlacesShips();
                //RevealEnemy();
                EnemyPlacesShips();
            } catch(Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace);
            }
        }

        /// <summary>
        /// Draws the grids with rectangles binded to appropriate cells for both the Player and Enemy
        /// </summary>
        public void FillGrids()
        {
            FillPlayerGrid();
            FillEnemyGrid();
        }

        /// <summary>
        /// Fills the PlayerShipAreaStackPanel with rectangles representing Tiles in the Player's Playgrid
        /// This method will clear and rebuild the PlayerShipStackPanel if called multiple times
        /// </summary>
        public void FillPlayerGrid()
        {
            PlayerShipAreaStackPanel.Children.Clear();
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
                            usedData.PreviewShipPlace(activePlaceShip, usedData.PlayerShipsGrid, mouseX, mouseY);
                        }
                        //MessageBox.Show($"{mouseX},{mouseY}");
                    };
                    r.MouseLeftButtonDown += DoPlaceShip;
                    r.MouseRightButtonDown += (sender, args) =>
                    {
                        activePlaceShip.IsVertical = !activePlaceShip.IsVertical;
                        usedData.RevertPreviewChanges(usedData.PlayerShipsGrid);
                        usedData.PreviewShipPlace(activePlaceShip, usedData.PlayerShipsGrid, mouseX, mouseY);
                    };
                    sp.Children.Add(r);
                }
                PlayerShipAreaStackPanel.Children.Add(sp);
            }
        }

        /// <summary>
        /// Fills the HitAreaStackPanel with rectangles representing Tiles in the Enemy's Playgrid
        /// This method will clear and rebuild the HitStackPanel if called multiple times
        /// </summary>
        public void FillEnemyGrid()
        {
            HitAreaStackPanel.Children.Clear();
            for (int i = 0; i < usedData.EnemyShipsGrid.Width; i++)
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
                    r.SetBinding(Rectangle.FillProperty, b);
                    sp.Children.Add(r);
                }
                HitAreaStackPanel.Children.Add(sp);
            }
        }

        /// <summary>
        /// Has the player place their ships, awaiting until all are placed
        /// </summary>
        public async void PlayerPlacesShips()
        {
            isPlacingShips = true;

            foreach (Ship s in usedData.PlayerShips)
            {
                activePlaceShip = s;
                while (activePlaceShip.Tiles == null) {
                    await WhenClicked(PlayerShipAreaStackPanel); //will repeated wait for each left mouse down until the ship is actually placed
                }
            }

            isPlacingShips = false;
            activePlaceShip = null;
        }

        /// <summary>
        /// A helper method acts as a pause until click delay
        /// </summary>
        /// <param name="target">A UIElement to await a leftbuttondown event on</param>
        /// <returns></returns>
        private static Task WhenClicked(UIElement target)
        {
            var tcs = new TaskCompletionSource<object>();
            MouseButtonEventHandler onClick = null;
            onClick = (sender, e) =>
            {
                target.MouseLeftButtonDown -= onClick;
                tcs.TrySetResult(null);
            };
            target.MouseLeftButtonDown += onClick;
            return tcs.Task;
        }

        /// <summary>
        /// The on left click action to place ships
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DoPlaceShip(object sender, RoutedEventArgs args)
        {
            if (activePlaceShip != null && isPlacingShips)
            {
                usedData.PlaceShip(activePlaceShip, usedData.PlayerShipsGrid
                    , mouseX > usedData.PlayerShipsGrid.Width - activePlaceShip.Length && !activePlaceShip.IsVertical //when the mouse is in the rightmost/bottommost spot, PlaceShip wont run from the IsValidPlacement check, so the altered 'edge' point will need to be sent in
                        ? usedData.PlayerShipsGrid.Width - activePlaceShip.Length
                        : mouseX
                    , mouseY > usedData.PlayerShipsGrid.Height - activePlaceShip.Length && activePlaceShip.IsVertical
                        ? usedData.PlayerShipsGrid.Height - activePlaceShip.Length
                        : mouseY
                );
            }
        }

        /// <summary>
        /// Click based action that reveals the enemy grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DoReveal(object sender, RoutedEventArgs args)
        {
            MenuItem revealMenuItem = (MenuItem)sender;
            if (t2bObf.IsObfuscated == true)
            {
                revealMenuItem.Header = "Hide Enemy Ships";
            }
            else
            {
                revealMenuItem.Header = "Reveal Enemy Ships";
            }
            t2bObf.IsObfuscated = !t2bObf.IsObfuscated;
            FillEnemyGrid(); //to 'refresh' the grid, since i can't really bind to a property/notify its change in the converter
        }

        /// <summary>
        /// Places Enemy ships onto the grid randomly
        /// </summary>
        public void EnemyPlacesShips()
        {
            Random rng = new Random();
            foreach (Ship s in usedData.EnemyShips)
            {
                int x = rng.Next(0, 10);
                int y = rng.Next(0, 10);
                s.IsVertical = rng.Next(0, 2) == 1 ? true : false;
                while (!usedData.IsValidPlacement(s,usedData.EnemyShipsGrid,x,y) || usedData.IsShipIntersecting(s,usedData.EnemyShipsGrid,x,y))
                {
                    x = rng.Next(0, 10);
                    y = rng.Next(0, 10);
                }
                usedData.PlaceShip(s,usedData.EnemyShipsGrid,x,y);
            }
        }
    }
}
