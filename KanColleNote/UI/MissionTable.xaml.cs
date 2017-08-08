using KanColleNote.Base;
using KanColleNote.Core;
using KanColleNote.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    /// MissionTable.xaml 的交互逻辑
    /// </summary>
    public partial class MissionTable : UserControl
    {

        dynamic root;

        public MissionTable()
        {
            InitializeComponent();
            GlobalNotification.Default.Register(NotificationType.kMissionUpdate, typeof(MissionTable), OnMissionUpdate);
            GlobalNotification.Default.Register(NotificationType.kKanMasterNameChange, typeof(MissionTable), OnKanMasterNameChange);
            //dataGridMission.ItemsSource = KanMission.m_mission;
        }

        void OnKanMasterNameChange(GlobalNotificationMessage msg)
        {
            //dataGridMission.ItemsSource = KanMission.m_mission;
        }

        void OnMissionUpdate(GlobalNotificationMessage msg)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                dataGridMission.ItemsSource = null;
                root = JsonConvert.DeserializeObject<dynamic>(KanMission.m_mission.ToString());
                Debug.WriteLine(KanMission.m_mission);
                dataGridMission.ItemsSource = root;

            })); 

        }
    }
}
