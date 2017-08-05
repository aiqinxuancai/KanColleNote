using LiveCharts;
using LiveCharts.Wpf;
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

namespace KanColleNote.UI
{
    /// <summary>
    /// MissionTable.xaml 的交互逻辑
    /// </summary>
    public partial class MissionTable : UserControl
    {

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }



        public MissionTable()
        {
            InitializeComponent();


            Labels = new[] { "0801", "0802", "0803", "0804", "0805" };

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "油",
                    PointForeground =  new SolidColorBrush(Color.FromRgb(84,168,72)),
                    Values = new ChartValues<double> { 300000, 280000, 200000, 220000, 250000 }
                },
                new LineSeries
                {
                    Title = "弹",
                    PointForeground =  new SolidColorBrush(Color.FromRgb(255,83,67)),
                    Values = new ChartValues<double> { 150000, 120000, 140000, 140000, 180000 }
                },
                new LineSeries
                {
                    Title = "钢",
                    PointForeground =  new SolidColorBrush(Color.FromRgb(190,190,190)),
                    Values = new ChartValues<double> { 152330, 110000, 158000, 140200, 171000 }
                },
                new LineSeries
                {
                    Title = "铝",
                    PointForeground =  new SolidColorBrush(Color.FromRgb(244,148,69)),
                    Values = new ChartValues<double> { 100000, 80000, 70000, 60000, 30000 }
                }
            };

            cartesianChart.SeriesColors = new ColorsCollection {
                Color.FromRgb(64,148,52),
                Color.FromRgb(235,63,47),
                Color.FromRgb(170,170,170),
                Color.FromRgb(224,128,49)
            };
            DataContext = this;
        }
    }
}
