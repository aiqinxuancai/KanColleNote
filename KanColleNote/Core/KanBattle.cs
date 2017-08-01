using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core
{
    class KanBattle
    {
        public static JObject m_lastStart;
        //public static JObject m_lastStart;
        //public static JObject m_lastStart;


        public static int m_lastPoint;





        public static bool SetStartData(string json)
        {
            try
            {
                m_lastStart = JObject.Parse(json);
                
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        //public static bool SetStartData(string json)
        //{
        //    try
        //    {
        //        m_lastStart = JObject.Parse(json);
        //        return true;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        return false;
        //    }
        //}

    }
}
