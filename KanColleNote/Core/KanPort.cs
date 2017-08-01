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
            	m_port = JObject.Parse(json);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

    }
}
