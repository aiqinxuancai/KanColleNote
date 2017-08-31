using KanColleNote.Core.Prophet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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
    /// ProphetTable.xaml 的交互逻辑
    /// </summary>
    public partial class ProphetTable : UserControl
    {
        public ProphetTable()
        {
            InitializeComponent();

            //JArray arr = new JArray();
            //JObject root = new JObject();
            //root["name"] = "长门改二";
            //root["hp"] = "60/90";
            //root["now"] = 20;
            //arr.Add(root);
            //arr.Add(root);
            //arr.Add(root);
            //arr.Add(root);
            //arr.Add(root);
            //arr.Add(root);
            //dataGridSlef1.ItemsSource = arr;


            BattleUnit item = new BattleUnit();
            item.name = "lalla";
            ArrayList arr = new ArrayList();
            arr.Add(item);
            arr.Add(item);
            arr.Add(item);
            arr.Add(item);
            arr.Add(item);
            arr.Add(item);

            dataGridSlef1.ItemsSource = arr;

            //dataGridSlef1.DataContext = arr;
            //负责数据源
        }
    }
}
