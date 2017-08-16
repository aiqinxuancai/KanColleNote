using KanColleNote.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanColleNote.Model;
using System.IO;

namespace KanColleNote.Core
{


    class KanDataCore
    {
        public static JObject m_start;


        public static bool SetGameStartData(string json)
        {
            try
            {
                m_start = JObject.Parse(json);
                File.WriteAllText(App.m_runPath + @"\Data\api_start2.json", json);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 从本地加载全局数据
        /// </summary>
        /// <returns></returns>
        public static bool LoadStartData()
        {
            try
            {
                if (File.Exists(App.m_runPath + @"\Data\api_start2.json"))
                {
                    var json = File.ReadAllText(App.m_runPath + @"\Data\api_start2.json");
                    m_start = JObject.Parse(json);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 根据船ID获取名字 包括深海船
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetKanNameWithId(int id)
        {
            JToken kanMstShip = m_start.SelectToken($"api_data.{KanDataType.SHIP}");

            if (kanMstShip != null)
            {
                JArray kanMstShipArray = (JArray)kanMstShip;
                JObject kanData = (JObject)kanMstShipArray.First(a => a["api_id"].Value<int>() == id);
                return kanData["api_name"].Value<string>();
            }

            return "";
        }

        /// <summary>
        /// 根据船ID获取Json
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static JObject GetKanJsonWithId(int id)
        {
            JToken kanMstShip = m_start.SelectToken($"api_data.{KanDataType.SHIP}");

            if (kanMstShip != null)
            {
                JArray kanMstShipArray = (JArray)kanMstShip;
                JObject kanData = (JObject)kanMstShipArray.First(a => a["api_id"].Value<int>() == id);
                return kanData;
            }

            return null;
        }

        /// <summary>
        /// 获取道具类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static JObject GetItemJsonWithId(int id)
        {
            JToken kanMstShip = m_start.SelectToken($"api_data.{KanDataType.SLOTITEM_EQUIPTYPE}");

            if (kanMstShip != null)
            {
                JArray kanMstShipArray = (JArray)kanMstShip;
                JObject kanData = (JObject)kanMstShipArray.First(a => a["api_id"].Value<int>() == id);
                return kanData;
            }

            return null;
        }

        /// <summary>
        /// 获取家具
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static JObject GetFurnitureWithId(int id)
        {
            var o = JsonHelper.GetKanDataWithId(m_start, KanDataType.FURNITURE, id);
            return  o;
        }


        public static JObject GetAnyWithId(string typeName, int id)
        {
            var o = JsonHelper.GetKanDataWithId(m_start, typeName, id);
            return o;
        }





    }
}
