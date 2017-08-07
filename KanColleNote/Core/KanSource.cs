using KanColleNote.Base;
using KanColleNote.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace KanColleNote.Core
{
    /// <summary>
    /// 记录资源情况
    /// </summary>
    class KanSource
    {

        public static JObject m_source;
        public static string m_savePath;


        

        static KanSource()
        {
            
            //GlobalNotification.Default.Register(NotificationType.kKanMasterNameChange, typeof(KanPort), OnKanMasterNameChange);
        }

        public static void Init ()
        {
            m_source = new JObject();
            GlobalNotification.Default.Register(NotificationType.kKanMasterNameChange, typeof(KanPort), OnKanMasterNameChange);
        }

        /// <summary>
        /// 收到一个玩家名称改变的消息 重载数据 用于多账号的统计
        /// </summary>
        /// <param name="msg"></param>
        public static void OnKanMasterNameChange(GlobalNotificationMessage msg)
        {
            //重新Load
            m_savePath = $@"{KanMaster.m_masterPath}\KanSource.json";
            if (File.Exists(m_savePath))
            {
                m_source = JObject.Parse(File.ReadAllText(m_savePath));
            }
        }

        public static void Save()
        {
            File.WriteAllText(m_savePath, m_source.ToString(Formatting.Indented));
            
        }


        public static void UpdateSource(JArray json)
        {
            if (json != null)
            {
                //需要重构，存储结构使用date作为key
                //记录
                var now = DateTime.Now;
                var date = now.ToShortDateString();

                JObject root = new JObject();
                root["date"] = date;
                root["time"] = now.Ticks / 1000;
                root["show_date"] = $"{now.Month}.{now.Day}";
                root["api_material"] = json;
                m_source[date] = root;
                Save();
                GlobalNotification.Default.Post(NotificationType.kSourceUpdate, null);
            }
        }


    }
}
