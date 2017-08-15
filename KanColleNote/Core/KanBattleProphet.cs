using KanColleNote.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core
{

    class KanBattleProphetHP
    {
        public List<int> m_nowhpsSelf;
        public List<int> m_nowhpsEnemy;

        public List<int> m_nowhps;
        public List<int> m_nowhps_combined;
        public KanBattleProphetHP(List<int> nowhps, List<int> nowhps_combined = null)
        {
            m_nowhps = nowhps;
            m_nowhpsSelf = m_nowhps.GetRange(1, 6);
            m_nowhpsEnemy = m_nowhps.GetRange(7, 6);
            if (nowhps_combined != null)
            {
                m_nowhps_combined = nowhps_combined;

                m_nowhpsSelf.AddRange(m_nowhps_combined.GetRange(1, 6));
                if (m_nowhps_combined.Count == 13)
                {
                    m_nowhpsEnemy.AddRange(m_nowhps_combined.GetRange(7, 6));
                }
            }
        }

        /// <summary>
        /// 更新自己队伍的血量
        /// </summary>
        /// <param name="updateHps"></param>
        /// <returns></returns>
        public void UpdateSelfHP(List<int> updateHps)
        {
            updateHps = HPListCheck(updateHps);
            for (int i = 0; i < updateHps.Count; i++)
            {
                m_nowhpsSelf[i] -= updateHps[i];
                if (updateHps[i] != 0)
                {
                    Debug.WriteLine($"我方{i} - {updateHps[i]}");
                }
            }
        }

        /// <summary>
        /// 更新自己第二队伍的血量
        /// </summary>
        /// <param name="updateHps"></param>
        public void UpdateSelfHPCombined(List<int> updateHps)
        {
            updateHps = HPListCheck(updateHps);
            for (int i = 6; i < m_nowhpsSelf.Count; i++)
            {
                m_nowhpsSelf[i] -= updateHps[i - 6];
                if (updateHps[i-6] != 0)
                {
                    Debug.WriteLine($"我方{i} - {updateHps[i - 6]}");
                }
            }
        }

        /// <summary>
        /// 更新敌方队伍的血量
        /// </summary>
        /// <param name="updateHps"></param>
        /// <returns></returns>
        public void UpdateEnemyHP(List<int> updateHps)
        {
            updateHps = HPListCheck(updateHps);
            for (int i = 0; i < updateHps.Count; i++)
            {
                m_nowhpsEnemy[i] -= updateHps[i];
                if(updateHps[i] != 0)
                {
                    Debug.WriteLine($"敌方{i} - {updateHps[i]}");
                }
            }
        }

        /// <summary>
        /// 更新敌方第二队伍的血量
        /// </summary>
        /// <param name="updateHps"></param>
        public void UpdateEnemyHPCombined(List<int> updateHps)
        {
            updateHps = HPListCheck(updateHps);
            for (int i = 6; i < m_nowhpsEnemy.Count; i++)
            {
                m_nowhpsEnemy[i] -= updateHps[i - 6];
                if (updateHps[i-6] != 0)
                {
                    Debug.WriteLine($"敌方{i} - {updateHps[i - 6]}");
                }
            }
        }

        public List<int> HPListCheck(List<int> updateHps)
        {
            if (updateHps.Count != 6 || updateHps.Count != 12)
            {
                updateHps = updateHps.GetRange(1, updateHps.Count - 1);  //删除掉第一个-1的值
            }
            return updateHps;
        }


        /// <summary>
        /// 更新炮击伤害
        /// </summary>
        /// <param name="df">谁攻击了谁</param>
        /// <param name="damage">造成伤害</param>
        /// <returns></returns>
        public void UpdateHouGeKiHP(JArray df, JArray damage, List<int> eflag = null, int selfTeamId = 1, int enemyTeamId = 1) //api_hougeki1
        {
            for (int i = 0; i < df.Count; i++)
            {
                var target = df[i];
                //子数组 比如二连则是俩 普通攻击则是一个
                if (target.Type == JTokenType.Array)
                {
                    JArray targetArray = (JArray)target;
                    for (int x = 0; x < targetArray.Count; x++)
                    {
                        int targetId = targetArray[x].Value<int>(); //攻击到谁
                        int damageHp = damage[i][x].Value<int>();   //减少多少血量

                        if (eflag != null) //有eflag的处理方式 联合舰队
                        {
                            //联合舰队战斗
                            if (eflag[i]  == 0) //由我方发起的攻击 减少对面的血量
                            {
                                m_nowhpsEnemy[targetId - 1] -= damageHp;
                                if (damageHp > 0)
                                {
                                    Debug.WriteLine($@"炮击 敌方：{targetId} 减去 {damageHp}");
                                }
                                
                            }
                            else if (eflag[i] == 1) //由敌方发起
                            {
                                m_nowhpsSelf[targetId - 1] -= damageHp;
                                if (damageHp > 0)
                                {
                                    Debug.WriteLine($@"炮击 我方：{targetId} 减去 {damageHp}");
                                }
                                
                            }
                        }
                        else
                        {
                            //普通战斗
                            if (targetId >= 1 && targetId <= 6)
                            {
                                //我方第一队伍
                                m_nowhpsSelf[targetId - 1 + (selfTeamId - 1) * 6] -= damageHp;
                                if (damageHp > 0)
                                {
                                    Debug.WriteLine($@"炮击 我方：{targetId} 减去 {damageHp}");
                                }

                            }
                            if (targetId >= 7 && targetId <= 12)
                            {
                                //敌方第一队伍
                                m_nowhpsEnemy[targetId - 7 + (enemyTeamId - 1) * 6] -= damageHp;
                                if (damageHp > 0)
                                {
                                    Debug.WriteLine($@"炮击 敌方：{targetId - 6} 减去 {damageHp}");
                                }
                            }
                        }
                        //m_nowhps[targetId] -= damageHp;
                    }
                }
            }
        }
    }




    /// <summary>
    /// 战斗先知 根据Battle来计算
    /// </summary>
    class KanBattleProphet
    {
        public static void test()
        {
            SetBattle(JObject.Parse(File.ReadAllText(@"D:\git\KanColleNote\KanColleNote\bin\Debug\pack配合录像115743\636383957358084-kcsapi-api_req_combined_battle-ec_midnight_battle.json")));
        }

        public static void SetBattle(JObject root)
        {
            //初始血量

           

            List<int> nowhpsList = JsonHelper.SelectTokenIntList(root, "api_data.api_nowhps");
            List<int> nowhpsListCombined = JsonHelper.SelectTokenIntList(root, "api_data.api_nowhps_combined");

            KanBattleProphetHP nowhps = new KanBattleProphetHP(nowhpsList, nowhpsListCombined);

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
                    List<int> api_fdam_combined = root.SelectToken("api_data.api_kouku.api_stage3_combined.api_fdam").ToObject<List<int>>();
                    List<int> api_edam_combined = root.SelectToken("api_data.api_kouku.api_stage3_combined.api_edam").ToObject<List<int>>();
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
                if (houraiFlag[0] == 1) //第一回合
                {
                    Debug.WriteLine("第一回合");
                    JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki1.api_df_list");
                    JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki1.api_damage");
                    List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki1.api_at_eflag");

                    nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_eflag);
                }

                if (houraiFlag[1] == 1) //第二回合
                {
                    Debug.WriteLine("第二回合");
                    JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki2.api_df_list");
                    JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki2.api_damage");
                    List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki2.api_at_eflag");
                    nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_eflag);
                }

                if (houraiFlag[2] == 1) //第三回合
                {
                    Debug.WriteLine("第三回合");
                    JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki3.api_df_list");
                    JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki3.api_damage");
                    List<int> api_at_eflag = JsonHelper.SelectTokenIntList(root, "api_data.api_hougeki3.api_at_eflag");
                    nowhps.UpdateHouGeKiHP(api_df_list, api_damage, api_at_eflag);
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


        }






    }
}
