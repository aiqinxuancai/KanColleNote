using KanColleNote.Base;
using KanColleNote.Core.Prophet;
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

    /// <summary>
    /// 战斗先知 根据Battle来计算
    /// </summary>
    class BattleProphet
    {
        public static void test()
        {
            var file = @"H:\舰队分析\packE7\636385929219884-kcsapi-api_req_combined_battle-battle.json";

            SetBattle(JObject.Parse(File.ReadAllText(file)), "api_req_combined_battle", "battle");
        }


        public static void SetBattle(string root, string from = "")
        {
            JObject json = JObject.Parse(root);
            SetBattle(json, from);
        }

        public static void SetBattle(JObject root, string type = "", string from = "")
        {
            //初始血量
            List<int> nowhpsList = JsonHelper.SelectTokenIntList(root, "api_data.api_nowhps");
            List<int> nowhpsListCombined = JsonHelper.SelectTokenIntList(root, "api_data.api_nowhps_combined");

            BattleHPManager nowhps = new BattleHPManager(nowhpsList, nowhpsListCombined);

            Debug.WriteLine("路基支援");
            //路基支援 估计是统一的掉血 N轮次
            JToken api_air_base_attack = root.SelectToken("api_data.api_air_base_attack");
            if (api_air_base_attack != null && api_air_base_attack.Type == JTokenType.Array)
            {
                JArray api_air_base_attack_array = (JArray)api_air_base_attack;

                for (int i = 0; i < api_air_base_attack_array.Count; i++)
                {
                    Debug.WriteLine($"第{i+1}轮路基");
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

            Debug.WriteLine("航空战");
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
            Debug.WriteLine("支援射击");
            //是否有支援 估计是统一的掉血
            int support = JsonHelper.SelectTokenInt(root, "api_data.api_support_flag");
            if (support == 1 || support == 2)
            {
                List<int> api_damage = root.SelectToken("api_data.api_support_info.api_support_hourai.api_damage").ToObject<List<int>>();
                nowhps.UpdateEnemyHP(api_damage);
            }

            //开幕反潜 估计和炮击战相似
            Debug.WriteLine("开幕反潜");
            int taisen = JsonHelper.SelectTokenInt(root, "api_data.api_opening_taisen_flag");
            if (taisen == 1)
            {
                JArray api_df_list = (JArray)root.SelectToken("api_data.api_opening_taisen.api_df_list");
                JArray api_damage = (JArray)root.SelectToken("api_data.api_opening_taisen.api_damage");
                nowhps.UpdateHouGeKiHP(api_df_list, api_damage);
            }




            Debug.WriteLine("开幕雷击");
            //是否有开幕雷击
            int opening = JsonHelper.SelectTokenInt(root, "api_data.api_opening_flag");
            if (opening == 1)
            {
                List<int> api_fdam = root.SelectToken("api_data.api_opening_atack.api_fdam").ToObject<List<int>>();
                List<int> api_edam = root.SelectToken("api_data.api_opening_atack.api_edam").ToObject<List<int>>();
                nowhps.UpdateSelfHP(api_fdam);
                nowhps.UpdateEnemyHP(api_edam);
            }

            //炮击回合 0-2  闭幕雷击 3
            List<int> houraiFlag = JsonHelper.SelectTokenIntList(root, "api_data.api_hourai_flag");

            if (houraiFlag != null) //白天的战斗
            {
                if (houraiFlag[0] == 1 && root.SelectToken("api_data.api_hougeki1") != null) //第一回合
                {
                    int selfTeam = 1;
                    if (type == "api_req_combined_battle" && from == "battle")
                    {
                        selfTeam = 2;
                    }

                    Debug.WriteLine("第一回合");
                    JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki1.api_df_list");
                    JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki1.api_damage");
                    List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki1.api_at_eflag");

                    nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_eflag, selfTeam);
                }

                if (houraiFlag[1] == 1 && root.SelectToken("api_data.api_hougeki2") != null) //第二回合
                {
                    int selfTeam = 1;
                    if (type == "api_req_combined_battle" && from == "battle")
                    {
                        selfTeam = 2;
                    }
                    Debug.WriteLine("第二回合");
                    JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki2.api_df_list");
                    JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki2.api_damage");
                    List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki2.api_at_eflag");
                    nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_eflag, selfTeam);
                }

                if (houraiFlag[2] == 1 && root.SelectToken("api_data.api_hougeki3") != null) //第三回合
                {
                    Debug.WriteLine("第三回合");
                    int selfTeam = 1;
                    if (type == "api_req_combined_battle" && from == "battle_water")
                    {
                        selfTeam = 2;
                    }
                    JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki3.api_df_list");
                    JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki3.api_damage");
                    List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki3.api_at_eflag");
                    nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_eflag, selfTeam);

                }
            }
            else
            {
                Debug.WriteLine("夜战炮击");
                List<int> api_active_deck = JsonHelper.SelectTokenIntList(root, "api_data.api_active_deck");
                if (api_active_deck != null)
                {
                    //联合舰队夜战
                    JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki.api_df_list");
                    JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki.api_damage");
                    List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki.api_at_eflag");
                    nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_eflag, api_active_deck[0], api_active_deck[1]);
                }
                else
                {
                    //夜战 普通的大概 没测
                    JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki.api_df_list");
                    JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki.api_damage");
                    List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki.api_at_eflag");
                    nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_eflag);
                }
            }

            Debug.WriteLine("闭幕雷击");
            //是否有闭幕雷击
            if (houraiFlag != null)
            {
                int raigeki = houraiFlag[3];
                if (raigeki == 1)
                {
                    List<int> api_fdam = root.SelectToken("api_data.api_raigeki.api_fdam").ToObject<List<int>>();
                    List<int> api_edam = root.SelectToken("api_data.api_raigeki.api_edam").ToObject<List<int>>();
                    nowhps.UpdateSelfHP(api_fdam);
                    nowhps.UpdateEnemyHP(api_edam);
                }
            }


            Debug.WriteLine(nowhps.m_nowhpsSelf.ToString());
            Debug.WriteLine(nowhps.m_nowhpsEnemy.ToString());

            //Debug.WriteLine("123".Substring(1, 1));

        }


        public static void hougeki(JObject root, BattleHPManager nowhps, string type = "", string from = "")
        {

        }





    }
}
