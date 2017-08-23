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
        [JsonProperty("api_id")]
        public string Id { get; set; }
        [JsonProperty("api_time")]
        public string Time { get; set; }
        [JsonProperty("api_quest_name")]
        public string QuestName { get; set; }
        [JsonProperty("api_get_material")]
        public List<int> Material { get; set; }
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
            GlobalNotification.Default.Register(NotificationType.kKanMasterIdChangeAfter, typeof(BattleTable), OnKanMasterIdChangeAfter);
            GlobalNotification.Default.Register(NotificationType.kMissionUpdate, typeof(MissionTable), OnMissionUpdate);
        }
        /// <summary>
        /// 名字变更完成后
        /// </summary>
        /// <param name="msg"></param>
        void OnKanMasterIdChangeAfter(GlobalNotificationMessage msg)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                dataGridMission.ItemsSource = null;
                root = JsonConvert.DeserializeObject<List<MissionData>>(KanMission.m_mission.ToString());
                dataGridMission.ItemsSource = root;
            }));

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
