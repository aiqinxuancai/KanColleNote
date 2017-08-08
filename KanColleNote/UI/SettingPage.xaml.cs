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
            SaveConfig();
        }

        private void SaveConfig()
        {
            JObject j = JObject.Parse(JsonConvert.SerializeObject(root));
            SpeedConfig.jsonTable.Merge(j);
            SpeedConfig.Save();
        }


        private async void buttonAutoSetProxy_Click(object sender, RoutedEventArgs e)
        {
            buttonAutoSetProxy.IsEnabled = false;
            AutoProxyService auto = new AutoProxyService();
            string data = await auto.AutoSetting();

            if (string.IsNullOrEmpty(data))
            {
                //失败
                MessageBox.Show("没有发现ACGPower、岛风Go、Shadowsocks进程！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                JObject json = JObject.Parse(data);


                root.ProxyIP = "127.0.0.1";
                root.ProxyPort = json["port"].Value<int>();
                //成功
                MessageBox.Show($"发现{json["type"]}，代理端口设置为：{json["port"]}", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                Debug.WriteLine(data);


                SaveConfig();
            }

            Debug.WriteLine("运行完毕");
            buttonAutoSetProxy.IsEnabled = true ;

        }
    }
}
