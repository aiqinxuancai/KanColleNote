using KanColleNote.Base;
using KanColleNote.Core.Prophet;
using KanColleNote.Model;
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


            //BattleUnit item = new BattleUnit();
            //item.name = "lalla";
            //item.maxHP = 100;
            //item.nowHP = 80;
            //ArrayList arr = new ArrayList();

            ////List<BattleRound> round = new List<BattleRound>() { new BattleRound() { message = "888888888888888" } };
            //item.round.Add(new BattleRound() { message = "1" });
            //item.round.Add(new BattleRound() { message = "2" });
            //item.round.Add(new BattleRound() { message = "3" });
            //item.round.Add(new BattleRound() { message = "4" });

            //arr.Add(item);
            //item.nowHP = 70;
            //arr.Add(item);
            //item.nowHP = 60;
            //arr.Add(item);
            //item.nowHP = 50;
            //arr.Add(item);
            //item.nowHP = 40;
            //arr.Add(item);
            //item.nowHP = 80;
            //arr.Add(item);

            //dataGridSlef1.ItemsSource = arr;

            //dataGridSlef1.DataContext = arr;
            //负责数据源


            GlobalNotification.Default.Register(NotificationType.kKanMasterIdChangeAfter, typeof(ProphetTable), OnKanMasterIdChangeAfter);
            GlobalNotification.Default.Register(NotificationType.kBattleProphetUpdate, typeof(ProphetTable), OnBattleProphetUpdate);

        }


        void OnKanMasterIdChangeAfter(GlobalNotificationMessage msg)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                dataGridSlef1.ItemsSource = null;
                dataGridSlef2.ItemsSource = null;
                dataGridEnemy1.ItemsSource = null;
                dataGridEnemy2.ItemsSource = null;
            }));
        }

        void OnBattleProphetUpdate(GlobalNotificationMessage msg)
        {
            BattleHPManager nowHps = (BattleHPManager)msg.Source;

            List<BattleUnit> selfTeam1 = nowHps.m_self.GetRange(0, 6);
            List<BattleUnit> selfTeam2 = nowHps.m_self.Count == 12 ? nowHps.m_self.GetRange(6, 6) : null;
            List<BattleUnit> enemyTeam1 = nowHps.m_enemy.GetRange(0, 6);
            List<BattleUnit> enemyTeam2 = nowHps.m_enemy.Count == 12 ? nowHps.m_enemy.GetRange(6, 6) : null;

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                dataGridSlef1.ItemsSource = null;
                dataGridSlef2.ItemsSource = null;
                dataGridEnemy1.ItemsSource = null;
                dataGridEnemy2.ItemsSource = null;

                dataGridSlef1.ItemsSource = selfTeam1;
                dataGridSlef2.ItemsSource = selfTeam2;
                dataGridEnemy1.ItemsSource = enemyTeam1;
                dataGridEnemy2.ItemsSource = enemyTeam2;
            }));
        }

    }
}
