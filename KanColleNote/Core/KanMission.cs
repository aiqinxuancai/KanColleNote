using KanColleNote.Base;
using KanColleNote.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core
{
    class KanMission
    {

        public static JArray m_mission;
        public static string m_savePath;


        public static void Init()
        {
            m_mission = new JArray();
            GlobalNotification.Default.Register(NotificationType.kKanMasterIdChange, typeof(KanMission), OnKanMasterNameChange);
        }
        public static void Save()
        {
            File.WriteAllText(m_savePath, m_mission.ToString(Formatting.Indented));

        }
        /// <summary>
        /// 收到一个玩家名称改变的消息 重载数据 用于多账号的统计
        /// </summary>
        /// <param name="msg"></param>
        public static void OnKanMasterNameChange(GlobalNotificationMessage msg)
        {
            //重新Load
            m_savePath = $@"{KanMaster.m_masterPath}\KanMission.json";
            if (File.Exists(m_savePath))
            {
                try
                {
                	m_mission = JArray.Parse(File.ReadAllText(m_savePath));
                    GlobalNotification.Default.Post(NotificationType.kMissionUpdate, null);
                }
                catch (System.Exception ex)
                {
                    m_mission = new JArray();
                }
                
            }
            else
            {
                m_mission = new JArray();
            }
        }

        public static void SetMissionResult(string json)
        {
            JObject root = JObject.Parse(json);
            root = (JObject)root["api_data"];
            root["api_time"] = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            root["api_id"] = m_mission.Count + 1;
            m_mission.AddFirst(root);
            Save();
            Debug.WriteLine("得到远征结果");
            Debug.WriteLine(root);
            GlobalNotification.Default.Post(NotificationType.kMissionUpdate, null);
        }



    }
}
