using KanColleNote.Base;
using KanColleNote.Model;
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

    /// <summary>
    /// 核心主类，用于存储获取母港状态
    /// </summary>
    class KanPort
    {
        public static JObject m_port;

        static KanPort()
        {
            m_port = new JObject();
            
        }



        /// <summary>
        /// 设置母港信息
        /// </summary>
        public static bool SetPortData(string json)
        {
            try
            {
                var port = JObject.Parse(json);
                JsonMergeSettings setting = new JsonMergeSettings();
                setting.MergeArrayHandling = MergeArrayHandling.Replace;
  
                m_port.Merge(port, setting);
                //更新名字等信息
                KanMaster.Update(m_port);
                //更新当前的资源情况
                KanSource.UpdateSource((JArray)m_port.SelectToken("api_data.api_material"));

                //File.WriteAllText(Directory.GetCurrentDirectory() + $@"\Merge{DateTime.Now.Ticks/1000/1000}.json", m_port.ToString());
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 启动时更新道具信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool UpdateRequireInfoData(string json)
        {
            try
            {
                var requireInfo = JObject.Parse(json);
                JsonMergeSettings setting = new JsonMergeSettings();
                setting.MergeArrayHandling = MergeArrayHandling.Replace;
                m_port.Merge(requireInfo, setting);
                //更新名字等信息
                KanMaster.StartUpdate(m_port);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static bool UpdateUnsetslotData(string json)
        {
            try
            {
                var info = JObject.Parse(json);
                m_port["api_data"]["api_unsetslot"] = info["api_data"];
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static bool UpdateUseitemData(string json)
        {
            try
            {
                var info = JObject.Parse(json);
                m_port["api_data"]["api_useitem"] = info["api_data"];
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static bool UpdateSlotitemData(string json)
        {
            try
            {
                var info = JObject.Parse(json);
                m_port["api_data"]["api_slot_item"] = info["api_data"];
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 战斗中更新船的信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool UpdateShipDeck(string json)
        {
            try
            {
                var info = JObject.Parse(json);
                MergeArrayFromAPIId("api_ship", info, "api_ship_data"); //合并船数据
                MergeArrayFromAPIId("api_deck_port", info, "api_deck_data"); //合并队伍数据？
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }


        public static bool MergeArrayFromAPIId(string rootKeyName, JObject json, string keyName)
        {
            if (m_port == null)
            {
                return false;
            }
            JArray rootArray = (JArray)m_port.SelectToken($"api_data.{rootKeyName}");
            JArray array = (JArray)json.SelectToken($"api_data.{keyName}");
            
            if (rootArray == null || array == null)
            {
                return false;
            }

            foreach (JObject item in array)
            {
                //bool change = false;
                uint apiId = item["api_id"].Value<uint>();
                for (int i = 0; i < rootArray.Count; i++)
                {
                    if (rootArray[i]["api_id"].Value<uint>() == apiId)
                    {
                        m_port["api_data"][rootKeyName][i] = item;
                        //change = true;
                        break;
                    }
                }

                //JToken kanMstShipTest = m_port.SelectToken($"$.api_data.{rootKeyName}.[?(@.api_id == {apiId})]");
                //m_port.SelectToken("").Path
            }
            return true;
        }

        /// <summary>
        /// 获取自己的一艘船
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static JToken GetShip(int id)
        {
            JToken kanShip = m_port.SelectToken($"$.api_data.api_ship.[?(@.api_id == {id})]", false);
            return kanShip;
        }

        public static JToken GetTeam(int id)
        {
            JObject kanTeam = (JObject)m_port.SelectToken($"$.api_data.api_deck_port.[?(@.api_id == {id})]", false);
            if (kanTeam == null )
            {
                return null;
            }
            JArray kanShips = (JArray)kanTeam.SelectToken("api_ship", false);
            if (kanShips == null)
            {
                return null;
            }
            JArray kanShipData = new JArray();
            for (int i=0; i < kanShips.Count; i ++)
            {
                JToken ship = GetShip(kanShips[0].Value<int>());
                ship["api_ship_data"] = KanDataCore.GetKanJsonWithId(ship["api_ship_id"].Value<int>());
                kanShipData.Add(ship);
            }
            kanTeam["api_ship_full"] = kanShipData;

            return kanTeam;
        }



    }
}
