using KanColleNote.Base;
using KanColleNote.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            m_source = new JArray();
            GlobalNotification.Default.Register(NotificationType.kKanMasterNameChange, typeof(KanPort), OnKanMasterNameChange);
        }
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

        public static void Reload()
        {
            m_source = new JArray();

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


                JObject o = (JObject)m_source.SelectToken($"$.[?(@.date == {date})]");
                if (o != null)
                {
                    o = root;
                }
                else
                {
                    m_source.Add(root);
                }

            }
        }


    }
}
