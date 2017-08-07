using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using LiveCharts;
using LiveCharts.Wpf;

namespace KanColleNote
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {



        public MainWindow()
        {

            InitializeComponent();


            DataContext = this;

            JArray array = new JArray();
            JObject test = new JObject();
            test["id"] = 100000000000;
            array.Add(test);

            array.Add(test);
            array.Add(test);
            array.Add(test);

            //dataGridMission.ItemsSource = array;
            //listViewMission.ItemsSource = array;


            Task.Run(() =>
            {
                Thread.Sleep(3000);
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    array.Add(test);


                }));

                Thread.Sleep(3000);
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    array[3]["wowo"] = "8888";


                }));
            });

        }
    }
}
