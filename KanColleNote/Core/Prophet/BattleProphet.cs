﻿using KanColleNote.Base;
using KanColleNote.Core.Prophet;
using KanColleNote.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core.Prophet
{

    public enum BattleTime
    {
        Day = 0,
        Night = 1
    }

    /// <summary>
    /// 战斗先知 根据Battle来计算
    /// </summary>
    class BattleProphet
    {
        public static void test()
        {
            var file = @"D:\git\KanColleNote\KanColleNote\PackData\packE5出击\636382390186510-kcsapi-api_req_sortie-ld_airbattle.json";

            //SetBattle(JObject.Parse(File.ReadAllText(file)), "api_req_combined_battle", "ec_battle");
        }


        public static void SetBattle(string root, string type = "", string from = "")
        {
            JObject json = JObject.Parse(root);
            SetBattle(json, type, from);
        }

        public static void SetBattle(JObject root, string type = "", string from = "")
        {
            Debug.WriteLine("SetBattle:" + type + " " + from);
            //初始血量
            if (KanDataCore.m_start == null || KanPort.m_port == null)
            {
                return;
            }

            BattleHPManager nowhps = new BattleHPManager(root);


            nowhps.SetEventName("路基支援");
            //路基支援 估计是统一的掉血 N轮次
            JToken api_air_base_attack = root.SelectToken("api_data.api_air_base_attack");
            if (api_air_base_attack != null && api_air_base_attack.Type == JTokenType.Array)
            {
                JArray api_air_base_attack_array = (JArray)api_air_base_attack;

                for (int i = 0; i < api_air_base_attack_array.Count; i++)
                {
                    nowhps.SetEventName($"第{i+1}轮路基");
                    JObject air_base = (JObject)api_air_base_attack_array[i];
                    if (air_base.SelectToken("api_stage3.api_edam") != null)
                    {
                        List<int> api_edam = JsonHelper.SelectTokenIntList(air_base, "api_stage3.api_edam");
                        nowhps.UpdateEnemyHP(api_edam);
                    }

                    if (air_base.SelectToken("api_stage3_combined.api_edam") != null)
                    {
                        List<int> api_edam = JsonHelper.SelectTokenIntList(air_base, "api_stage3_combined.api_edam");
                        nowhps.UpdateEnemyHPCombined(api_edam);
                    }
                }
            }

            nowhps.SetEventName("航空战");
            //航空战 是否造成伤害
            int stage = JsonHelper.SelectTokenInt(root, "api_data.api_stage_flag[2]");
            if (stage == 1)
            {
                //计算航空伤害
                List<int> api_fdam = root.SelectToken("api_data.api_kouku.api_stage3.api_fdam").ToObject<List<int>>();
                List<int> api_edam = root.SelectToken("api_data.api_kouku.api_stage3.api_edam").ToObject<List<int>>();
                nowhps.UpdateSelfHP(api_fdam);
                nowhps.UpdateEnemyHP(api_edam);

                if (root.SelectToken("api_data.api_kouku.api_stage3_combined.api_fdam") != null)
                {
                    List<int> api_fdam_combined = JsonHelper.SelectTokenIntList(root, "api_data.api_kouku.api_stage3_combined.api_fdam");
                    List<int> api_edam_combined = JsonHelper.SelectTokenIntList(root, "api_data.api_kouku.api_stage3_combined.api_edam");
                    nowhps.UpdateSelfHPCombined(api_fdam_combined);
                    nowhps.UpdateEnemyHPCombined(api_edam_combined);
                }
            }

            nowhps.SetEventName("支援射击");
            //是否有支援 估计是统一的掉血
            int support = JsonHelper.SelectTokenInt(root, "api_data.api_support_flag");
            if (support == 1 || support == 2)
            {
                List<int> api_damage = root.SelectToken("api_data.api_support_info.api_support_hourai.api_damage").ToObject<List<int>>();
                nowhps.UpdateEnemyHP(api_damage);
            }

            //开幕反潜 估计和炮击战相似
            nowhps.SetEventName("开幕反潜");
            int taisen = JsonHelper.SelectTokenInt(root, "api_data.api_opening_taisen_flag");
            if (taisen == 1)
            {
                JArray api_df_list = (JArray)root.SelectToken("api_data.api_opening_taisen.api_df_list", false);
                JArray api_damage = (JArray)root.SelectToken("api_data.api_opening_taisen.api_damage", false);
                JArray api_at_list = (JArray)root.SelectToken("api_data.api_opening_taisen.api_at_list", false);
                JArray api_at_type = (JArray)root.SelectToken("api_data.api_opening_taisen.api_at_type", false);
                nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_list, api_at_type, BattleTime.Day);
            }

            nowhps.SetEventName("开幕雷击");
            //是否有开幕雷击
            int opening = JsonHelper.SelectTokenInt(root, "api_data.api_opening_flag");
            if (opening == 1)
            {
                List<int> api_fdam = root.SelectToken("api_data.api_opening_atack.api_fdam").ToObject<List<int>>();
                List<int> api_edam = root.SelectToken("api_data.api_opening_atack.api_edam").ToObject<List<int>>();
                nowhps.UpdateSelfHP(api_fdam);
                nowhps.UpdateEnemyHP(api_edam);
            }

            //计算炮击和闭幕雷击 
            if (type == "api_req_combined_battle" && from == "battle") //机动部队x敌单舰队
            {
                HoGeKiManeuver(root, nowhps);
            }
            else if (type == "api_req_combined_battle" && from == "battle_water") //水打部队x敌单舰队
            {
                HoGeKiWater(root, nowhps);
            }
            else
            {
                HoGeKiNormal(root, nowhps);
            }
            string self = "";
            nowhps.m_nowhpsSelf.ForEach(x => self += "," + x);
            string enemy = "";
            nowhps.m_nowhpsEnemy.ForEach(x => enemy += "," + x);

   
            Debug.WriteLine("我方血量：" + self);
            Debug.WriteLine("敌方血量：" + enemy);

            //Debug.WriteLine("123".Substring(1, 1));

            GlobalNotification.Default.Post(NotificationType.kBattleProphetUpdate, nowhps);

        }


        
        public static void HoGeKiManeuver(JObject root, BattleHPManager nowhps)
        {
            
            List<int> houraiFlag = JsonHelper.SelectTokenIntList(root, "api_data.api_hourai_flag");

            if (houraiFlag != null) //白天的战斗
            {
                if (houraiFlag[0] == 1 && root.SelectToken("api_data.api_hougeki1", false) != null) //第一回合
                {
                    HoGeKiBase(root, nowhps, 1, 2);
                }
                if (houraiFlag[1] == 1)
                {
                    nowhps.SetEventName("闭幕雷击");
                    List<int> api_fdam = root.SelectToken("api_data.api_raigeki.api_fdam").ToObject<List<int>>();
                    List<int> api_edam = root.SelectToken("api_data.api_raigeki.api_edam").ToObject<List<int>>();
                    nowhps.UpdateSelfHP(api_fdam);
                    nowhps.UpdateEnemyHP(api_edam);
                }
                if (houraiFlag[2] == 1 && root.SelectToken("api_data.api_hougeki2", false) != null) //第二回合
                {
                    HoGeKiBase(root, nowhps, 2, 1);
                }
                if (houraiFlag[3] == 1 && root.SelectToken("api_data.api_hougeki3", false) != null) //第三回合
                {
                    HoGeKiBase(root, nowhps, 3, 1);
                }

            }
            else
            {
                HoGeKiNight(root, nowhps);
            }
        }

        public static void HoGeKiWater(JObject root, BattleHPManager nowhps)
        {
            List<int> houraiFlag = JsonHelper.SelectTokenIntList(root, "api_data.api_hourai_flag");

            if (houraiFlag != null) //白天的战斗
            {
                if (houraiFlag[0] == 1 && root.SelectToken("api_data.api_hougeki1", false) != null) //第一回合
                {
                    HoGeKiBase(root, nowhps, 1);
                }
                if (houraiFlag[1] == 1 && root.SelectToken("api_data.api_hougeki2", false) != null) //第二回合
                {
                    HoGeKiBase(root, nowhps, 2);
                }
                if (houraiFlag[2] == 1 && root.SelectToken("api_data.api_hougeki3", false) != null) //第三回合
                {
                    HoGeKiBase(root, nowhps, 3, 2);
                }
                int raigeki = houraiFlag[3];
                if (raigeki == 1)
                {
                    nowhps.SetEventName("闭幕雷击");
                    List<int> api_fdam = root.SelectToken("api_data.api_raigeki.api_fdam").ToObject<List<int>>();
                    List<int> api_edam = root.SelectToken("api_data.api_raigeki.api_edam").ToObject<List<int>>();
                    nowhps.UpdateSelfHP(api_fdam);
                    nowhps.UpdateEnemyHP(api_edam);
                }
            }
            else
            {
                HoGeKiNight(root, nowhps);
            }
        }


        public static void HoGeKiNormal(JObject root, BattleHPManager nowhps)
        {
            List<int> houraiFlag = JsonHelper.SelectTokenIntList(root, "api_data.api_hourai_flag");

            if (houraiFlag != null) //白天的战斗
            {
                if (houraiFlag[0] == 1 && root.SelectToken("api_data.api_hougeki1", false) != null) //第一回合
                {
                    HoGeKiBase(root, nowhps, 1);
                }
                if (houraiFlag[1] == 1 && root.SelectToken("api_data.api_hougeki2", false) != null) //第二回合
                {
                    HoGeKiBase(root, nowhps, 2);
                }
                if (houraiFlag[2] == 1 && root.SelectToken("api_data.api_hougeki3", false) != null) //第三回合
                {
                    HoGeKiBase(root, nowhps, 3);
                }

                int raigeki = houraiFlag[3];
                if (raigeki == 1)
                {
                    nowhps.SetEventName("闭幕雷击");
                    List<int> api_fdam = root.SelectToken("api_data.api_raigeki.api_fdam").ToObject<List<int>>();
                    List<int> api_edam = root.SelectToken("api_data.api_raigeki.api_edam").ToObject<List<int>>();
                    nowhps.UpdateSelfHP(api_fdam);
                    nowhps.UpdateEnemyHP(api_edam);
                }
            }
            else
            {
                HoGeKiNight(root, nowhps);
            }

        }

        public static void HoGeKiBase(JObject root, BattleHPManager nowhps, int round, int selfTeamId = 1)
        {
            nowhps.SetEventName($"第{round}回合炮击");
            JArray api_df_list = (JArray)root.SelectToken($"api_data.api_hougeki{round}.api_df_list", false);
            JArray api_damage = (JArray)root.SelectToken($"api_data.api_hougeki{round}.api_damage", false);
            JArray api_at_list = (JArray)root.SelectToken($"api_data.api_hougeki{round}.api_at_list", false);
            JArray api_at_type = (JArray)root.SelectToken($"api_data.api_hougeki{round}.api_at_type", false);

            List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, $"api_data.api_hougeki{round}.api_at_eflag");
            nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_list, api_at_type, BattleTime.Day, api_at_eflag, selfTeamId);
        }


        public static void HoGeKiNight(JObject root, BattleHPManager nowhps)
        {
            nowhps.SetEventName($"夜战炮击");
            List<int> api_active_deck = JsonHelper.SelectTokenIntList(root, "api_data.api_active_deck");
            if (api_active_deck != null)
            {
                
                //联合舰队夜战
                JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki.api_df_list", false);
                JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki.api_damage", false);
                JArray api_at_list = (JArray)root.SelectToken($"api_data.api_hougeki.api_at_list", false);
                JArray api_at_type = (JArray)root.SelectToken($"api_data.api_hougeki.api_at_type", false); //白天CI类型 0=普通 1=？？ 2=连击 3=主炮副炮 4=主炮电探 5=主炮撤甲 6=主炮主炮
                JArray api_sp_list = (JArray)root.SelectToken($"api_data.api_hougeki.api_sp_list", false); //CI类型 0=普通 1=连击 3=鱼雷鱼雷 4=主炮鱼雷 5=主炮主炮
                List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki.api_at_eflag");
                nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_list, api_sp_list, BattleTime.Night, api_at_eflag, api_active_deck[0], api_active_deck[1]);
            }
            else
            {
                //夜战 普通的大概 没测
                JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki.api_df_list", false);
                JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki.api_damage", false);
                JArray api_at_list = (JArray)root.SelectToken($"api_data.api_hougeki.api_at_list", false);
                JArray api_at_type = (JArray)root.SelectToken($"api_data.api_hougeki.api_at_type", false); //白天CI类型 0=普通 1=？？ 2=连击 3=主炮副炮 4=主炮电探 5=主炮撤甲 6=主炮主炮
                JArray api_sp_list = (JArray)root.SelectToken($"api_data.api_hougeki.api_sp_list", false); //CI类型 0=普通 1=连击 3=鱼雷鱼雷 4=主炮鱼雷 5=主炮主炮
                List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki.api_at_eflag");
                nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_list, api_sp_list, BattleTime.Night, api_at_eflag);
            }
        }

    }
}
