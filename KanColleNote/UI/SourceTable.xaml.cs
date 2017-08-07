using KanColleNote.Base;
using KanColleNote.Core;
using KanColleNote.Model;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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

namespace KanColleNote.UI
{
    /// <summary>
    /// MissionTable.xaml 的交互逻辑
    /// </summary>
    public partial class SourceTable : UserControl
    {

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }



        public SourceTable()
        {
            InitializeComponent();

            GlobalNotification.Default.Register(NotificationType.kSourceUpdate, typeof(KanPort), OnSourceUpdate);


            
        }

        /// <summary>
        /// 收到资源信息有变更
        /// </summary>
        /// <param name="msg"></param>
        void OnSourceUpdate(GlobalNotificationMessage msg)
        {
            //更新资源图表数据
            var properties = KanSource.m_source.Properties().ToList();
            var label = new ArrayList();
            var source1 = new ArrayList(); //油
            var source2 = new ArrayList(); //蛋
            var source3 = new ArrayList(); //钢
            var source4 = new ArrayList(); //铝

            foreach (var item in properties)
            {
                label.Add(item.Value["show_date"].ToObject<string>());
                source1.Add(item.Value["api_material"].First(a => a["api_id"].Value<int>() == 1)["api_value"].ToObject<int>());
                source2.Add(item.Value["api_material"].First(a => a["api_id"].Value<int>() == 2)["api_value"].ToObject<int>());
                source3.Add(item.Value["api_material"].First(a => a["api_id"].Value<int>() == 3)["api_value"].ToObject<int>());
                source4.Add(item.Value["api_material"].First(a => a["api_id"].Value<int>() == 4)["api_value"].ToObject<int>());
            }

            Labels = (string[])label.ToArray(typeof(string));
            this.Dispatcher.BeginInvoke(new Action(() =>
            {

                    SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "油",
                        PointForeground =  new SolidColorBrush(Color.FromRgb(84,168,72)),
                        Values = new ChartValues<int>((int[])source1.ToArray(typeof(int)))
                    },
                    new LineSeries
                    {
                        Title = "弹",
                        PointForeground =  new SolidColorBrush(Color.FromRgb(255,83,67)),
                        Values = new ChartValues<int>((int[])source2.ToArray(typeof(int)))
                    },
                    new LineSeries
                    {
                        Title = "钢",
                        PointForeground =  new SolidColorBrush(Color.FromRgb(190,190,190)),
                        Values = new ChartValues<int>((int[])source3.ToArray(typeof(int)))
                    },
                    new LineSeries
                    {
                        Title = "铝",
                        PointForeground =  new SolidColorBrush(Color.FromRgb(244,148,69)),
                        Values = new ChartValues<int>((int[])source4.ToArray(typeof(int)))
                    }
                };

                    cartesianChart.SeriesColors = new ColorsCollection {
                    Color.FromRgb(64,148,52),
                    Color.FromRgb(235,63,47),
                    Color.FromRgb(170,170,170),
                    Color.FromRgb(224,128,49)
                };
                DataContext = this;
                //this.UpdateLayout();
                //cartesianChart.UpdateLayout();
            }));


        }

    }
}
