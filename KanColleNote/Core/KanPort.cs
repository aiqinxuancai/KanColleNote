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
            GlobalNotification.Default.Register(NotificationType.kKanMasterNameChange, typeof(KanPort), OnKanMasterNameChange);
        }

        public static void OnKanMasterNameChange(GlobalNotificationMessage msg)
        {
            //重新Load
        }

        /// <summary>
        /// 设置母港信息
        /// </summary>
        public static bool SetPortData(string json)
        {
            try
            {
            	m_port = JObject.Parse(json);
                //更新名字等信息
                KanMaster.Update(m_port);
                //更新资源
                KanSource.UpdateSource((JArray)m_port.SelectToken("api_data.api_material"));


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
