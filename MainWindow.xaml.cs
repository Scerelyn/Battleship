using BattleShip.Logical;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public MainWindow()
        {
            InitializeComponent();
            //GameData d = GameData.LoadData(Directory.GetCurrentDirectory() + "/file.dat");//new GameData();
            //d.SaveData(Directory.GetCurrentDirectory() + "/file.dat");
            //MessageBox.Show(d.PlayerShips.ValueAt(1,1)+"");
        }
    }
}
