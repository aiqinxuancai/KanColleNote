using KanColleNote.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core
{
    class KanDataCore
    {
        public static JObject m_start;

        /// <summary>
        /// 船
        /// </summary>
        const string KAN_DATA_TYPE_SHIP = "api_mst_ship";
        //const string KAN_DATA_TYPE_SHIP = "api_mst_shipgraph"; //船图像，对应的swf
        //const string KAN_DATA_TYPE_SHIP = "api_mst_slotitem_equiptype"; //道具类型
        //const string KAN_DATA_TYPE_SHIP = "api_mst_equip_exslot"; //??
        //const string KAN_DATA_TYPE_SHIP = "api_mst_equip_exslot_ship"; //??
        //const string KAN_DATA_TYPE_SHIP = "api_mst_stype";    //船类型
        //const string KAN_DATA_TYPE_SHIP = "api_mst_slotitem"; //装备
        //const string KAN_DATA_TYPE_SHIP = "api_mst_furniture"; //家具

        //const string KAN_DATA_TYPE_SHIP = "api_mst_furnituregraph";
        //const string KAN_DATA_TYPE_SHIP = "api_mst_useitem"; //可使用的道具，开发资材，战斗粮食等
        //const string KAN_DATA_TYPE_SHIP = "api_mst_payitem"; //收费道具，伊良湖等
        //const string KAN_DATA_TYPE_SHIP = "api_mst_item_shop"; //商店排序，记录两页的显示数据，一般不可使用
        //const string KAN_DATA_TYPE_SHIP = "api_mst_maparea"; //区域，南方海域...

        
        //const string KAN_DATA_TYPE_SHIP = "api_mst_mapinfo"; //地图6-3等等api_maparea_id-api_no
        //const string KAN_DATA_TYPE_SHIP = "api_mst_mapbgm"; //地图BGM
        //const string KAN_DATA_TYPE_SHIP = "api_mst_mission"; //远征

        //const string KAN_DATA_TYPE_SHIP = "api_mst_const"; //？？？
        //const string KAN_DATA_TYPE_SHIP = "api_mst_shipupgrade"; //？？？
        //const string KAN_DATA_TYPE_SHIP = "api_mst_bgm"; //背景音乐，可以取得歌曲的名字
        

        public static bool SetGameStartData(string json)
        {
            try
            {
                m_start = JObject.Parse(json);
                KAN_DATA_TYPE_SHIP
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
            JToken kanMstShip = m_start.SelectToken("api_data.api_mst_ship");

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
            JToken kanMstShip = m_start.SelectToken("api_data.api_mst_ship");

            if (kanMstShip != null)
            {
                JArray kanMstShipArray = (JArray)kanMstShip;
                JObject kanData = (JObject)kanMstShipArray.First(a => a["api_id"].Value<int>() == id);
                return kanData;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>可在api_name中获取</returns>
        public static JObject GetItemJsonWithId(int id)
        {
            JToken kanMstShip = m_start.SelectToken("api_data.api_mst_slotitem_equiptype");

            if (kanMstShip != null)
            {
                JArray kanMstShipArray = (JArray)kanMstShip;
                JObject kanData = (JObject)kanMstShipArray.First(a => a["api_id"].Value<int>() == id);
                return kanData;
            }

            return null;
        }

        /// <summary>
        /// 家具
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static JObject GetFurnitureWithId(int id)
        {
            var o = JsonHelper.GetKanDataWithId(m_start, "api_mst_furniture", id);
            return  o;
        }


        public static JObject GetAnyWithId(string typeName, int id)
        {
            var o = JsonHelper.GetKanDataWithId(m_start, typeName, id);
            return o;
        }





    }
}
