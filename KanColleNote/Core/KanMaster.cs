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

        public static void StartUpdate(JObject json)
        {
            JObject master = (JObject)json.SelectToken("api_data.api_basic");
            if (master != null)
            {
                bool changeId = false;
                var userId = json.SelectToken("api_data.api_basic.api_member_id").Value<uint>();
                if (userId != member_id)
                {
                    member_id = userId;
                    changeId = true;
                }

                CreateMasterDirectory(member_id);
                if (changeId)
                {
                    GlobalNotification.Default.Post(NotificationType.kKanMasterIdChange, member_id);
                }
            }

        }


        public static void Update(JObject json)
        {
            JObject master = (JObject)json.SelectToken("api_data.api_basic");
            if (master != null)
            {
                bool changeName = false;
                var userId = json.SelectToken("api_data.api_basic.api_member_id").Value<uint>();
                if (userId != member_id)
                {
                    //名称有变更？ 使用通知？
                    changeName = true;
                }

                member_id = json.SelectToken("api_data.api_basic.api_member_id").Value<uint>();
                nickname = json.SelectToken("api_data.api_basic.api_nickname").Value<string>();
                nickname_id = json.SelectToken("api_data.api_basic.api_nickname_id").Value<uint>();
                starttime = json.SelectToken("api_data.api_basic.api_starttime").Value<ulong>();

                CreateMasterDirectory(member_id);
                if (changeName)
                {
                    GlobalNotification.Default.Post(NotificationType.kKanMasterIdChange, member_id);
                    GlobalNotification.Default.Post(NotificationType.kKanMasterIdChangeAfter, member_id);
                }
            }
        }


        public static void CreateMasterDirectory(uint userId)
        {
            var savePath = $@"{App.m_runPath}\Data\{userId}";
            if (Directory.Exists(savePath) == false)
            {
                Directory.CreateDirectory(savePath);
            }
            m_masterPath = savePath;
        }
    }
}
