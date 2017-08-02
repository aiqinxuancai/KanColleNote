using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Base
{
    class JsonHelper
    {
        public static JObject GetKanDataWithId(JObject root, string typeName, int id)
        {
            JToken kanMstShipTest = root.SelectToken($"$.api_data.{typeName}.[?(@.api_id == {id})]");
            return (JObject)kanMstShipTest;
        }


        public static JObject GetKanDataWithId2(JObject root, string typeName, int id)
        {
            JToken kanMstShip = root.SelectToken($"api_data.{typeName}");
            if (kanMstShip != null)
            {
                JArray kanMstShipArray = (JArray)kanMstShip;
                JObject kanData = (JObject)kanMstShipArray.First(a => a["api_id"].Value<int>() == id);
                return kanData;
            }
            return null;
        }

    }
}
