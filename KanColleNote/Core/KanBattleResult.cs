using KanColleNote.Base;
using KanColleNote.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core
{
    public class BattleData
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string Map { get; set; }
        public string MapPoint { get; set; }
        public string WinRank { get; set; }
        public string Ship { get; set; }
        /// <summary>
        /// 敌方舰队名称
        /// </summary>
        public string DeckName { get; set; }
        
    }


    class KanBattleResult
    {
        public static ArrayList m_battle;
        public static string m_savePath;

        public static void Init()
        {
            m_battle = new ArrayList();
            GlobalNotification.Default.Register(NotificationType.kKanMasterIdChange, typeof(KanMission), OnKanMasterNameChange);
        }
        public static void Save()
        {
            File.WriteAllText(m_savePath, JsonConvert.SerializeObject(m_battle));
        }
        /// <summary>
        /// 收到一个玩家名称改变的消息 重载数据 用于多账号的统计
        /// </summary>
        /// <param name="msg"></param>
        public static void OnKanMasterNameChange(GlobalNotificationMessage msg)
        {
            //重新Load
            m_savePath = $@"{KanMaster.m_masterPath}\KanBattleResult.json";
            if (File.Exists(m_savePath))
            {
                try
                {
                    m_battle = JsonConvert.DeserializeObject<ArrayList>(File.ReadAllText(m_savePath));
                    //m_battle = JArray.Parse(File.ReadAllText(m_savePath));
                    //GlobalNotification.Default.Post(NotificationType.kMissionUpdate, null);
                }
                catch (System.Exception ex)
                {
                    m_battle = new ArrayList();
                }

            }
            else
            {
                m_battle = new ArrayList();
            }

            //通知界面更新绑定
            GlobalNotification.Default.Post(NotificationType.kBattleResultBindingUpdate, null);
        }



        public static void SetBattleResult(BattleData data)
        {
            data.Time = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            data.Id = m_battle.Count + 1;
            m_battle.Insert(0, data);
            Save();
            Debug.WriteLine("得到战斗结果");
            Debug.WriteLine(data);
            
        }
    }
}
