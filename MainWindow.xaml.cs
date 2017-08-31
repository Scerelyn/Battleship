using BattleShip.Converters;
using BattleShip.Logical;
using BattleShip.Logical.AI;
using BattleShip.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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
        bool isPlacingShips = false, isGameRunning = false;
        int playerAreaMouseX = 0, playerAreaMouseY = 0, hitAreaMouseX = 0, hitAreaMouseY = 0;
        Ship activePlaceShip = null;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                NewGame(true,true);
            } catch(Exception e)
            {
                MessageBox.Show($"An error occured, here's some messages and stack traces:\n{e.Message} {e.StackTrace}","Error Occured",MessageBoxButton.OK,MessageBoxImage.Error);
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
                        playerAreaMouseX = x;
                        playerAreaMouseY = y;
                        if (isPlacingShips)
                        {
                            usedData.RevertPreviewChanges(usedData.PlayerShipsGrid);
                            usedData.PreviewShipPlace(activePlaceShip, usedData.PlayerShipsGrid, playerAreaMouseX, playerAreaMouseY);
                        }
                        //MessageBox.Show($"{mouseX},{mouseY}");
                    };
                    r.MouseLeftButtonDown += DoPlaceShip;
                    r.MouseRightButtonDown += (sender, args) =>
                    {
                        if (isPlacingShips)
                        {
                            activePlaceShip.IsVertical = !activePlaceShip.IsVertical;
                            usedData.RevertPreviewChanges(usedData.PlayerShipsGrid);
                            usedData.PreviewShipPlace(activePlaceShip, usedData.PlayerShipsGrid, playerAreaMouseX, playerAreaMouseY);
                        }
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
                int x = i;
                StackPanel sp = new StackPanel();
                for (int j = 0; j < usedData.EnemyShipsGrid.Height; j++)
                {
                    int y = j;
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
                    //Fire on click logic here
                    r.MouseLeftButtonDown += async (sender, args) =>
                    {
                        if (isGameRunning)
                        {
                            TileState aimed = ((Tile)r.DataContext).State; //need to lambda because of this line refering to the rectangle
                            if (aimed != TileState.Hit && aimed != TileState.Missed)
                            {
                                TurnIdentifierLabel.Content = "Enemy Turn";
                                TurnIdentifierLabel.Background = Brushes.IndianRed; //Make the turn label very obvious
                                HitAreaStackPanel.IsEnabled = false; //because of Delay(), the player could make another turn, so disable the stackpanel to prevent this from calling from another rectangle
                                bool youHit = usedData.Shoot(x, y, usedData.EnemyShipsGrid);
                                usedData.LogInfo = usedData.LogInfo + $"The Player hit point {ToBattleshipPoint(x, y)}";
                                usedData.LogInfo = usedData.LogInfo + (youHit ? "\nThe Player hits a ship!\n" : "\n");
                                Ship youMaybeHit = usedData.GetJustSank(usedData.EnemyShips);
                                if (youMaybeHit != null)
                                {
                                    usedData.LogInfo = usedData.LogInfo + $" !! The Player sank the enemy's {youMaybeHit.Name}\n";
                                    MessageBox.Show($"You just sank the enemy's {youMaybeHit.Name}!");
                                }
                                bool playerWin = GameEnd();
                                
                                if (!playerWin) { //AI moves if the game didnt end ie the player did not win
                                    await Delay(new Random().Next(1500)); //Random time to delay to make the AI seem like its 'thinking'
                                    Logical.Point p = usedData.ActiveAI.ChoosePoint();
                                    bool enemyHit = usedData.Shoot(p.X, p.Y, usedData.PlayerShipsGrid);
                                    usedData.LogInfo = usedData.LogInfo + $"The Enemy hit point {ToBattleshipPoint(p.X, p.Y)}";
                                    usedData.LogInfo = usedData.LogInfo + (enemyHit ? "\nThe Enemy hits a ship!\n" : "\n");
                                    Ship enemyMaybeHit = usedData.GetJustSank(usedData.PlayerShips);
                                    if (enemyMaybeHit != null)
                                    {
                                        usedData.LogInfo = usedData.LogInfo + $" !! The Enemy sank the player's {enemyMaybeHit.Name}\n";
                                        MessageBox.Show($"The enemy sank your {enemyMaybeHit.Name}!");
                                    }
                                    LogScrollView.ScrollToBottom();

                                    TurnIdentifierLabel.Content = "Your turn";
                                    TurnIdentifierLabel.Background = Brushes.LawnGreen;
                                    HitAreaStackPanel.IsEnabled = true;
                                    GameEnd();
                                }
                            }
                            else
                            {
                                MessageBox.Show("You already fired here");
                            }
                        }
                    };
                    r.MouseEnter += (sender, args) =>
                    {
                        hitAreaMouseX = x;
                        hitAreaMouseY = y;
                    };
                    sp.Children.Add(r);
                }
                HitAreaStackPanel.Children.Add(sp);
            }
        }

        /// <summary>
        /// Has the player place their ships, awaiting until all are placed
        /// </summary>
        public async Task PlayerPlacesShips()
        {
            isPlacingShips = true;
            FileMenuItem.IsEnabled = false;
            TurnIdentifierLabel.Content = "Place your Ships!";
            TurnIdentifierLabel.Background = Brushes.LightGoldenrodYellow;
            foreach (Ship s in usedData.PlayerShips)
            {
                activePlaceShip = s;
                while (activePlaceShip.Tiles == null) {
                    await WhenClicked(PlayerShipAreaStackPanel); //will repeatedly wait for each left mouse down until the ship is actually placed
                }
            }

            isPlacingShips = false;
            activePlaceShip = null;
            FileMenuItem.IsEnabled = true;
            TurnIdentifierLabel.Content = "Your turn";
            TurnIdentifierLabel.Background = Brushes.LawnGreen;
        }

        /// <summary>
        /// A helper method acts as a pause until click delay
        /// </summary>
        /// <param name="target">A UIElement to await a leftbuttondown event on</param>
        /// <returns></returns>
        private static Task WhenClicked(UIElement target)
        {
            //code below is borrowed and modified from the top answer from
            //https://stackoverflow.com/questions/35514733/wait-until-a-click-event-has-been-fired-c-sharp
            var tcs = new TaskCompletionSource<object>();
            MouseButtonEventHandler onClick = null;
            onClick = (sender, e) =>
            {
                target.MouseLeftButtonDown -= onClick;
                tcs.TrySetResult(null);
            };
            target.MouseLeftButtonDown += onClick;
            return tcs.Task;
            //end borrowed code
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
                    , playerAreaMouseX > usedData.PlayerShipsGrid.Width - activePlaceShip.Length && !activePlaceShip.IsVertical //when the mouse is in the rightmost/bottommost spot, PlaceShip wont run from the IsValidPlacement check, so the altered 'edge' point will need to be sent in
                        ? usedData.PlayerShipsGrid.Width - activePlaceShip.Length
                        : playerAreaMouseX
                    , playerAreaMouseY > usedData.PlayerShipsGrid.Height - activePlaceShip.Length && activePlaceShip.IsVertical
                        ? usedData.PlayerShipsGrid.Height - activePlaceShip.Length
                        : playerAreaMouseY
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

        /// <summary>
        /// The click based action called to save the usedData GameData instance to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DoSave(object sender, RoutedEventArgs args)
        {
            if (HitAreaStackPanel.Children.Count <= 10 || PlayerShipAreaStackPanel.Children.Count <= 10) //not 10x10 means the data is somehow invalid or empty
            {
                MessageBox.Show("Cannot save an empty grid!","Missing Grid data",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "Save";
                sfd.DefaultExt = ".btl";
                sfd.Filter = "Battleship Saves (.btl)|*.btl";
                bool? result = sfd.ShowDialog();
                string filepath = "";
                if ((result ?? false) == true)
                {
                    filepath = sfd.FileName;
                    Stream stream = File.Open(filepath, FileMode.Create);
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, usedData);
                    stream.Close();
                    MessageBox.Show("File successfully saved!","Save Success");
                }
            }
        }

        /// <summary>
        /// The click based action called to load a GameData instance from a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DoLoad(object sender, RoutedEventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Battleship Saves (.btl)|*.btl";
            bool? result = ofd.ShowDialog();
            string filepath = "";
            if ((result ?? false) == true)
            {
                filepath = ofd.FileName;
                Stream stream = File.Open(filepath, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                usedData = (GameData)formatter.Deserialize(stream);
                usedData.ActiveAI.PlayerGrid = usedData.PlayerShipsGrid;
                stream.Close();
                FillGrids();
                MessageBox.Show("File successfully Loaded!");
            }
        }

        /// <summary>
        /// The click based action called to start up a new game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DoNewGame(object sender, RoutedEventArgs args)
        {
            NewGame(false);
        }

        /// <summary>
        /// Creates a new game
        /// </summary>
        /// <param name="loadFileToo">A bool telling whether or not to have the load file option. True (default) to show it, false to hide it</param>
        /// <param name="isStartUp">A bool telling if this is the startup call, which determines if closing this will shutdown the program. True if it is, false (default) if it is not</param>
        public async void NewGame(bool loadFileToo = true, bool isStartUp = false)
        {
            TurnIdentifierLabel.Content = "New game setup";
            TurnIdentifierLabel.Background = Brushes.LightYellow;
            PlayerShipAreaStackPanel.Children.Clear();
            HitAreaStackPanel.Children.Clear();
            LogTextBlock.Text = "";
            usedData = new GameData();
            NewGameSettingsWindow ngsw = new NewGameSettingsWindow(loadFileToo);
            ngsw.AIChoiceComboBox.ItemsSource = new List<IAIModel>()
            {
                new EasyAI(usedData.PlayerShipsGrid),
                new MediumAI(usedData.PlayerShipsGrid),
                new HardAI(usedData.PlayerShipsGrid),
                new CheatingAI(usedData.PlayerShipsGrid),
            };
            bool? result = ngsw.ShowDialog();
            if ((result ?? false) == true)
            {
                if (ngsw.GameData != null)
                {
                    //implies that a file was loaded
                    usedData = ngsw.GameData;
                    this.Title = "Battleship AI Level: " + usedData.ActiveAI.ToString();
                    TurnIdentifierLabel.Content = "Your turn";
                    TurnIdentifierLabel.Background = Brushes.LawnGreen;
                    HitAreaStackPanel.IsEnabled = true;
                }
                else
                {
                    usedData.ActiveAI = ngsw.ChoosenAI;
                    this.Title = "Battleship AI Level: " + usedData.ActiveAI.ToString();
                    FillPlayerGrid();
                    HitAreaStackPanel.Children.Add(new TextBlock() { Text = "Place ships into your area on the other, blue grid area\nRight click to rotate the ship\nThe game will automatically start when all ships are placed\n\n\nClick in this area to shoot at the enemy when playing" });
                    await PlayerPlacesShips(); //async because the ships should be placed before the enemy grid is filled up
                    FillEnemyGrid();
                    EnemyPlacesShips();
                }
                isGameRunning = true;
                usedData.ActiveAI.PlayerGrid = usedData.PlayerShipsGrid;
                LogStackPanel.DataContext = usedData;
                Binding b = new Binding("LogInfo");
                LogTextBlock.SetBinding(TextBlock.TextProperty, b);
                FillGrids();
            }
            else
            {
                HitAreaStackPanel.Children.Add(new Label() { Content = "Go to file and make a new game, or close the window" });
                PlayerShipAreaStackPanel.Children.Add(new Label() { Content = "Go to file and make a new game or close the window" });
                TurnIdentifierLabel.Content = "Make a new game";
                TurnIdentifierLabel.Background = Brushes.Orange;
                if (isStartUp)
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// The method that handles behavior on game end
        /// </summary>
        /// <returns>True if the game ended, false if it did not</returns>
        public bool GameEnd()
        {
            int winner = usedData.WhoIsWinner();
            if (winner == 1) //player win
            {
                TurnIdentifierLabel.Content = "Game ended";
                TurnIdentifierLabel.Background = Brushes.MidnightBlue;
                if(MessageBox.Show("You win! Play again?", "You am victory", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    NewGame();
                }
                else
                {
                    PlayerShipAreaStackPanel.Children.Clear();
                    HitAreaStackPanel.Children.Clear();
                    PlayerShipAreaStackPanel.Children.Add(new Label() { Content="Go to file and make a new game!"});
                }
            }
            else if (winner == 2) //enemy win
            {
                TurnIdentifierLabel.Content = "Game ended";
                TurnIdentifierLabel.Background = Brushes.MidnightBlue;
                if (MessageBox.Show("You lost. Play again?", "You're lose", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    NewGame();
                }
                else
                {
                    PlayerShipAreaStackPanel.Children.Clear();
                    HitAreaStackPanel.Children.Clear();
                    PlayerShipAreaStackPanel.Children.Add(new Label() { Content = "Go to file and make a new game!" });
                }
            }
            return winner != 0;
        }

        /// <summary>
        /// Rotates the two grids to and from the vertial and horizontal positions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DoRotate(object sender, RoutedEventArgs args)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (MainStackPanel.Orientation == Orientation.Vertical)
            {
                MainStackPanel.Orientation = Orientation.Horizontal;
                menuItem.Header = "Change to Vertical View";
            }
            else
            {
                MainStackPanel.Orientation = Orientation.Vertical;
                menuItem.Header = "Change to Horizontal View";
            }
        }

        /// <summary>
        /// Converts a pair of cartisian points and converts it into the Battleship equivalent
        /// </summary>
        /// <param name="x">The x coordinate of the point to convert</param>
        /// <param name="y">The y coordinate of the point to convert</param>
        /// <returns>A string of the given coordinates in [letter]-[number] format that Battleship uses</returns>
        private string ToBattleshipPoint(int x, int y)
        {
            string BSPoint = "";
            BSPoint += (char)(65 + y);
            BSPoint += "-" + (x+1);
            return BSPoint;
        }

        /// <summary>
        /// Changes the visibility of the event log stackpanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DoLogVisChange(object sender, RoutedEventArgs args)
        {
            MenuItem visChanger = (MenuItem)sender;
            if (LogStackPanel.Visibility == Visibility.Hidden)
            {
                visChanger.Header = "Hide Event Log";
                LogStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                visChanger.Header = "Show Event Log";
                LogStackPanel.Visibility = Visibility.Hidden;
            }
            
        }

        /// <summary>
        /// An async method that causes a basic time based delay
        /// </summary>
        /// <param name="delay">Milliseconds to delay</param>
        public async Task Delay(int delay)
        {
            await Task.Run(
                () => { Thread.Sleep(delay); }
            );
        }
    }
}
