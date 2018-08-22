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
using KanColleNote.Core;
using KanColleNote.Core.Prophet;
using KanColleNote.Base;
using System.IO;
using CefSharp;

namespace KanColleNote
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {



        public MainWindow()
        {
            InitChromeBrowser();
            InitializeComponent();

            DataContext = this;
            



            //JArray array = new JArray();
            //JObject test = new JObject();
            //test["id"] = 100000000000;
            //array.Add(test);

            //array.Add(test);
            //array.Add(test);
            //array.Add(test);

            ////dataGridMission.ItemsSource = array;
            ////listViewMission.ItemsSource = array;


            //Task.Run(() =>
            //{
            //    Thread.Sleep(3000);
            //    this.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        array.Add(test);


            //    }));

            //    Thread.Sleep(3000);
            //    this.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        array[3]["wowo"] = "8888";


            //    }));
            //});

        }
        ~MainWindow()
        {
            
        }

        private void InitChromeBrowser()
        {
            var setting = new CefSettings()
            {
                CachePath = Directory.GetCurrentDirectory() + @"\Cache",
            };
            setting.Locale = "zh-CN";
            setting.CefCommandLineArgs.Add("enable-npapi", "1");
            setting.CefCommandLineArgs.Add("--proxy-server", "http://127.0.0.1:" + SpeedConfig.Get<int>(ConfigName.CONFIG_PROXY_SELFPORT, 37180));
            setting.CefCommandLineArgs.Add("--enable-media-stream", "1");
            setting.CefCommandLineArgs.Add("enable-media-stream", "1");

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            CefSharpSettings.ShutdownOnExit = true;



            if (!Cef.Initialize(setting))
            {
                throw new Exception("Unable to Initialize Cef");
            }

            
        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Test.KanTest.StartTest();
            //BattleProphet.test();
#endif
            //嵌入提督忙




        }
    }
}
