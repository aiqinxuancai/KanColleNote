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

        public static JArray m_source;
        public static string m_savePath;


        

        static KanSource()
        {
            
            //GlobalNotification.Default.Register(NotificationType.kKanMasterNameChange, typeof(KanPort), OnKanMasterNameChange);
        }

        public static void Init ()
        {
            m_source = new JArray();
            GlobalNotification.Default.Register(NotificationType.kKanMasterNameChange, typeof(KanPort), OnKanMasterNameChange);
        }

        /// <summary>
        /// 收到一个玩家名称改变的消息 重载数据 用于多账号的统计
        /// </summary>
        /// <param name="msg"></param>
        public static void OnKanMasterNameChange(GlobalNotificationMessage msg)
        {
            //重新Load
            string name = (string)msg.Source;
            var savePath = $@"{App.m_runPath}\Data\{name}";

            if (Directory.Exists(savePath) == false)
            {
                Directory.CreateDirectory(savePath);
            }
            m_savePath = $@"{savePath}\KanSource.json";
            if (File.Exists(m_savePath))
            {
                m_source = JArray.Parse(File.ReadAllText(m_savePath));
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
                //root.SelectToken($"$.api_data.{typeName}.[?(@.api_id == {id})]");
                //记录
                var now = DateTime.Now;
                var date = now.ToShortDateString();

                JObject root = new JObject();
                root["date"] = date;
                root["show_date"] = $"{now.Month}.{now.Day}";
                root["api_material"] = json;

                JToken o = new JObject();
                if (m_source.Count != 0)
                {
                    o = m_source.First(m => ((JObject)m).Property("date") != null && m["date"].Value<string>().Equals(date));
                }
                
                if (o.HasValues)
                {
                    m_source.
                    o = root;
                }
                else
                {
                    m_source.Add(root);
                }

                Save();

            }
        }


    }
}
