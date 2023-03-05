using System;
using System.Collections.Generic;
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

namespace Lotus.Windows.App
{
    public class CData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }
        public DateOnly Birthday { get; set; }
    }

    public static class CDataSeed
    {
        public static CData[] Datas = new CData[]
        {
            new CData { Name = "Даниил", Description = "Хороший", Age = 38, Birthday = new DateOnly(1984, 9, 18)},
            new CData { Name = "Михаил", Description = "Злой", Age = 35, Birthday = new DateOnly(1986, 9, 18)},
            new CData { Name = "Гаврииил", Description = "Плохой", Age = 42, Birthday = new DateOnly(1985, 9, 18)},
        };
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            lotusData.ItemsSource = CDataSeed.Datas;
        }
    }
}
