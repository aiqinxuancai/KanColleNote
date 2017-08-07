using KanColleNote.Base;
using KanColleNote.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            GlobalNotification.Default.Register(NotificationType.kKanMasterNameChange, typeof(KanPort), OnKanMasterNameChange);
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
                m_mission = JArray.Parse(File.ReadAllText(m_savePath));
            }
        }

        public static void SetMissionResult(string json)
        {
            JObject root = JObject.Parse(json);
            root = (JObject)root["api_data"];
            root["api_time"] = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            root["api_id"] = m_mission.Count + 1;
            m_mission.AddFirst(root);
        }



    }
}
