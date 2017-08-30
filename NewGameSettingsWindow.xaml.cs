using BattleShip.Logical;
using BattleShip.Logical.AI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BattleShip
{
    /// <summary>
    /// Interaction logic for NewGameSettingsWindow.xaml
    /// </summary>
    public partial class NewGameSettingsWindow : Window
    {
        List<IAIModel> AIList = null;
        public GameData GameData { get; private set; } = null;
        public IAIModel ChoosenAI { get; private set; } = null;
        public NewGameSettingsWindow(bool showLoad)
        {
            InitializeComponent();
            StartGameButton.Click += DoStartGame;
            if (!showLoad)
            {
                LoadFileButton.Visibility = Visibility.Hidden;
            }
            else
            {
                LoadFileButton.Click += DoLoad;
            }
        }

        private void DoStartGame(object sender, RoutedEventArgs e)
        {
            ChoosenAI = (IAIModel)AIChoiceComboBox.SelectedValue;
            this.DialogResult = true;
            this.Close();
        }

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
                GameData = (GameData)formatter.Deserialize(stream);
                stream.Close();
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
