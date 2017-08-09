using KanColleNote.Base;
using KanColleNote.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                m_port.Merge(port);
                //更新名字等信息
                KanMaster.Update(m_port);
                //更新当前的资源情况
                KanSource.UpdateSource((JArray)m_port.SelectToken("api_data.api_material"));


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
        public static bool SetRequireInfoData(string json)
        {
            try
            {
                var requireInfo = JObject.Parse(json);
                m_port.Merge(requireInfo);
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

        public static bool SetUnsetslotData(string json)
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

        public static bool SetUseitemData(string json)
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

        public static bool SetSlotitemData(string json)
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

        




    }
}
