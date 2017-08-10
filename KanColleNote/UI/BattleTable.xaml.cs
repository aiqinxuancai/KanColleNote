using KanColleNote.Base;
using KanColleNote.Core;
using KanColleNote.Model;
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
    /// BattleTable.xaml 的交互逻辑
    /// </summary>
    public partial class BattleTable : UserControl
    {
        public BattleTable()
        {
            InitializeComponent();
            GlobalNotification.Default.Register(NotificationType.kBattleResultBindingUpdate, typeof(BattleTable), OnBattleResultBindingUpdate);
        }



        void OnBattleResultBindingUpdate(GlobalNotificationMessage msg)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                dataGridBattle.ItemsSource = null;
                dataGridBattle.ItemsSource = KanBattleResult.m_battle;
            }));

        }
    }
}
