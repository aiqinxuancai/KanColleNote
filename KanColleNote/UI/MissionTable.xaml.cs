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



    public class MissionData
    {
        public string api_id { get; set; }
        public string api_time { get; set; }
        public string api_quest_name { get; set; }
        [JsonProperty("api_get_material")]
        public List<int> api_get_material { get; set; }
    }


    /// <summary>
    /// MissionTable.xaml 的交互逻辑
    /// </summary>
    public partial class MissionTable : UserControl
    {

        List<MissionData> root;

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

                root = JsonConvert.DeserializeObject<List<MissionData>>(KanMission.m_mission.ToString());

                dataGridMission.ItemsSource = root;

            })); 

        }
    }
}
