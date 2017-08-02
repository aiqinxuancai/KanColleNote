using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Mission
{
    class KanMission
    {




        public static void SetMissionResult(string json)
        {
            JObject root = JObject.Parse(json);
            JArray arr = new JArray();
            //arr.GetEnumerator


        }



    }
}
