using KanColleNote.Core;
using Nekoxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KanColleNote.Core.Prophet;

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

            Debug.WriteLine(obj.Request.PathAndQuery);
            Debug.WriteLine(fileName);

            Task.Run(() => {


                try
                {
                	RecvRoute(obj.Request.PathAndQuery, json);
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine("处理错误:" + fileName);
                    Debug.WriteLine(ex);
                }

            });

            //
            return false;

        }

        //const string BATTLE_TYPE_BATTLE


        public static bool RecvRoute(string path, string json)
        {
            var subPaths = path.Split(@"/".ToCharArray());
            var subPath = "";
            if (subPaths.Length > 0)
            {
                subPath = subPaths[subPaths.Length - 1]; //如api_start2这样的字串
            }

            switch (path) //战斗包处理
            {
                case @"/kcsapi/api_req_sortie/battle": //普通战斗 ok
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_combined_battle/battle": //激动舰队x单舰队 第二队伤害单独计算
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_combined_battle/battle_water": //水打舰队x单舰队 第二队伤害单独计算
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_combined_battle/ec_battle": //单舰队x联合舰队 ok 
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_combined_battle/each_battle_water": //水打x联合舰队 ok
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_combined_battle/each_battle": //机动x联合舰队 ok
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_combined_battle/ec_midnight_battle": //联合舰队夜战 ok
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case @"/kcsapi/api_req_battle_midnight/battle": //戦闘(夜戦) 
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_battle_midnight/sp_midnight": //戦闘(夜戦)
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_sortie/night_to_day": //夜战->昼战
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case @"/kcsapi/api_req_sortie/airbattle": //航空战
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_sortie/ld_airbattle": //长距离航空战 
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_combined_battle/airbattle": //航空战
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                case @"/kcsapi/api_req_combined_battle/ld_airbattle": //长距离航空战
                    KanBattle.SetBattleData(json);
                    BattleProphet.SetBattle(json, subPath);
                    return true;
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            }



            switch (path)
            {
                case @"/kcsapi/api_port/port": //回到母港
                    KanPort.SetPortData(json);
                    break;
                case @"/kcsapi/api_start2": //初始化游戏所有数据
                    KanDataCore.SetGameStartData(json);
                    break;
                case @"/kcsapi/api_get_member/questlist": //任务
                    break;
                case @"/kcsapi/api_get_member/mapinfo": //点击出击后 可用于更新地图信息
                    break;
                case @"/kcsapi/api_req_map/start": //出击开始
                    KanBattle.SetStartData(json);
                    break;
                case @"/kcsapi/api_req_sortie/battleresult": //战斗结果
                    KanBattle.SetBattleResultData(json);
                    break;
                case @"/kcsapi/api_req_combined_battle/battleresult": //战斗结果 联合舰队
                    KanBattle.SetBattleResultData(json);
                    break;
                case @"/kcsapi/api_req_map/next": //Next （合并更新到start里面去）
                    KanBattle.SetNextData(json);
                    break;
                case @"/kcsapi/api_get_member/ship_deck": //战斗中的船状态刷新
                    KanPort.UpdateShipDeck(json);
                    break;
                case @"/kcsapi/api_get_member/slot_item": //战斗结束 更新道具
                    KanPort.SetSlotitemData(json);
                    break;
                case @"/kcsapi/api_get_member/useitem":   //战斗结束 更新使用道具
                    KanPort.SetUseitemData(json);
                    break;
                case @"/kcsapi/api_get_member/unsetslot": //战斗结束 道具更新？？？？？
                    KanPort.SetUnsetslotData(json);
                    break;
                case @"/kcsapi/api_req_mission/result": //远征结果
                    KanMission.SetMissionResult(json);
                    break;

            }
            return true;
        }



    }
}
