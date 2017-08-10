using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static JToken SelectTokenPath(JObject root, string path)
        {
            try
            {
            	return root.SelectToken(path);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static int SelectTokenInt(JObject root, string path, int def = 0)
        {
            try
            {
                return root.SelectToken(path).Value<int>(); ;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return def;
            }
        }

        public static string SelectTokenString(JObject root, string path, string def = "")
        {
            try
            {
                return root.SelectToken(path).Value<string>();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return def;
            }
        }

    }
}
