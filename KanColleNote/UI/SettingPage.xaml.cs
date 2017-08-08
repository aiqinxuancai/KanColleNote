using KanColleNote.Base;
using KanColleNote.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : UserControl
    {
        dynamic root;


        public SettingPage()
        {
            InitializeComponent();
            root = JsonConvert.DeserializeObject<dynamic>(SpeedConfig.jsonTable.ToString());
            DataContext = root;
        }



        private void buttonSetting_Click(object sender, RoutedEventArgs e)
        {
            JObject j = JObject.Parse(JsonConvert.SerializeObject(root));
            SpeedConfig.jsonTable.Merge(j);
            SpeedConfig.Save();
        }

        private async void buttonAutoSetProxy_Click(object sender, RoutedEventArgs e)
        {
            AutoProxyService auto = new AutoProxyService();
            await auto.AutoSetting();
            Debug.WriteLine("运行完毕");
            

        }
    }
}
