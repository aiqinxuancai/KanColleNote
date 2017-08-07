using KanColleNote.Core;
using Nekoxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace KanColleNote
{
    class DataRoute
    {
        public static bool RecvPack(Session obj)
        {
            //Pack分发
            //obj.Response.Headers
            

            var jsons = obj.Response.BodyAsString.Split("=".ToCharArray(), 2);
            if (jsons.Length < 2)
            {
                Debug.WriteLine("不符合规范的游戏数据包");
                return false;
            }
            var json = jsons[1];


            if (Directory.Exists(App.m_runPath + @"\pack") == false)
            {
                Directory.CreateDirectory(App.m_runPath + @"\pack");
            }

            var fileName = App.m_runPath + @"\pack\"  + DateTime.Now.Ticks / 1000 + obj.Request.PathAndQuery.Replace("/", "-") + ".json";

            File.WriteAllText(fileName, json);

            switch (obj.Request.PathAndQuery)
            {
                case @"/kcsapi/api_port/port": //回到母港
                    KanPort.SetPortData(json);
                    break;
                case @"/kcsapi/api_start2": //初始化游戏所有数据
                    KanDataCore.SetGameStartData(json);
                    break;
                case @"/kcsapi/api_get_member/questlist": //任务
                    break;
                case @"/kcsapi/api_get_member/mapinfo": //点击出击后
                    break;
                case @"/kcsapi/api_req_map/start": //出击开始
                    KanBattle.SetStartData(json);
                    break;
                case @"/kcsapi/api_req_sortie/battleresult": //战斗结果
                    break;
                case @"/kcsapi/api_req_sortie/battle": //战斗
                    break;
                case @"/kcsapi/api_req_map/next": //Next （可以合并更新到start里面去？）
                    break;
                case @"/kcsapi/api_get_member/ship_deck": //战斗中的船检测
                    break;
                case @"/kcsapi/api_req_mission/result": //远征结果
                    KanMission.SetMissionResult(json);
                    break;
                    

            }







            return false;

        }

    }
}
