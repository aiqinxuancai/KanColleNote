using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core
{
    /// <summary>
    /// 记录资源情况
    /// </summary>
    class KanSource
    {

        public static JArray m_source;

        static KanSource()
        {
            m_source = new JArray();
        }

        public static void Reload()
        {
            //m_source = new JArray();

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
