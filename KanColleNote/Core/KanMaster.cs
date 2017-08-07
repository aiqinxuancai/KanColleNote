using KanColleNote.Base;
using KanColleNote.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core
{
    class KanMaster
    {
        public static string nickname { set; get; }
        public static uint member_id { set; get; }
        public static uint nickname_id { set; get; }
        public static ulong starttime { set; get; }

        public static string m_masterPath { set; get; }

        public static void Update(JObject json)
        {
            JObject master = (JObject)json.SelectToken("api_data.api_basic");
            if (master != null)
            {
                bool changeName = false;
                var nn = json.SelectToken("api_data.api_basic.api_nickname").Value<string>();

                if (nn.Equals(nickname) == false)
                {
                    //名称有变更？ 使用通知？
                    changeName = true;
                }

                member_id = json.SelectToken("api_data.api_basic.api_member_id").Value<uint>();
                nickname = json.SelectToken("api_data.api_basic.api_nickname").Value<string>();
                nickname_id = json.SelectToken("api_data.api_basic.api_nickname_id").Value<uint>();
                starttime = json.SelectToken("api_data.api_basic.api_starttime").Value<ulong>();

                CreateMasterDirectory(nn);
                if (changeName)
                {
                    GlobalNotification.Default.Post(NotificationType.kKanMasterNameChange, nickname);
                }
            }
        }


        public static void CreateMasterDirectory(string name)
        {
            var savePath = $@"{App.m_runPath}\Data\{name}";
            if (Directory.Exists(savePath) == false)
            {
                Directory.CreateDirectory(savePath);
            }
            m_masterPath = savePath;
        }
    }
}
