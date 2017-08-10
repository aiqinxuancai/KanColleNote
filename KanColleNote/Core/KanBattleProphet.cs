using KanColleNote.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core
{

    class KanBattleProphetHP
    {
        public List<int> m_nowhps;
        public KanBattleProphetHP(List<int> nowhps)
        {
            m_nowhps = nowhps;
        }

        /// <summary>
        /// 更新自己队伍的血量
        /// </summary>
        /// <param name="nowhps"></param>
        /// <param name="updateHps"></param>
        /// <returns></returns>
        public void UpdateSelfHP(List<int> updateHps)
        {
            for (int i = 0; i < updateHps.Count; i++)
            {
                int updateHp = updateHps[i];
                if (updateHp != -1)
                {
                    m_nowhps[i] -= updateHps[i];
                }
            }
        }

        /// <summary>
        /// 更新敌方队伍的血量
        /// </summary>
        /// <param name="nowhps"></param>
        /// <param name="updateHps"></param>
        /// <returns></returns>
        public void UpdateEnemyHP(List<int> updateHps)
        {
            var size = m_nowhps.Count - updateHps.Count;
            for (int i = m_nowhps.Count - 1; i > 0; i--)
            {
                if (i - size == -1)
                {
                    break;
                }
                int updateHp = updateHps[i - size];
                if (updateHp != -1)
                {
                    m_nowhps[i] -= updateHps[i - size];
                }
            }
        }

        /// <summary>
        /// 更新炮击伤害
        /// </summary>
        /// <param name="df">谁攻击了谁</param>
        /// <param name="damage">造成伤害</param>
        /// <returns></returns>
        public void UpdateHouGeKiHP(JArray df, JArray damage) //api_hougeki1
        {
            for (int i = 0; i < df.Count; i++)
            {
                var target = df[i];
                if (target.Type == JTokenType.Array)
                {
                    JArray targetArray = (JArray)target;
                    for (int x = 0; x < targetArray.Count; x++)
                    {
                        Debug.WriteLine(targetArray[x].Value<int>() + " 减去 " + damage[i][x].Value<int>());
                        m_nowhps[targetArray[x].Value<int>()] -= damage[i][x].Value<int>();
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
        public static void SetBattle(JObject root)
        {
            //初始血量
            List<int> nowhpsList = root.SelectToken("api_data.api_nowhps").ToObject<List<int>>();
            KanBattleProphetHP nowhps = new KanBattleProphetHP(nowhpsList);

            //航空战 是否造成伤害
            int stage = JsonHelper.SelectTokenInt(root, "api_data.api_stage_flag[2]");
            if (stage == 1)
            {
                //计算航空伤害
                List<int> api_fdam = root.SelectToken("api_data.api_kouku.api_stage3.api_fdam").ToObject<List<int>>();
                List<int> api_edam = root.SelectToken("api_data.api_kouku.api_stage3.api_edam").ToObject<List<int>>();
                nowhps.UpdateSelfHP(api_fdam);
                nowhps.UpdateEnemyHP(api_edam);
            }

            //是否有支援 估计是统一的掉血
            int support = JsonHelper.SelectTokenInt(root, "api_data.api_support_flag");

            //路基支援 估计是统一的掉血 N轮次


            //开幕反潜 估计和炮击战相似

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
            List<int> houraiFlag = root.SelectToken("api_data.api_hourai_flag").ToObject<List<int>>();
            if (houraiFlag[0] == 1) //第一回合
            {
                JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki1.api_df_list");
                JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki1.api_damage");
                nowhps.UpdateHouGeKiHP(api_df_list, api_damage);
            }

            if (houraiFlag[1] == 1) //第二回合
            {
                JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki2.api_df_list");
                JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki2.api_damage");
                nowhps.UpdateHouGeKiHP(api_df_list, api_damage);
            }

            if (houraiFlag[2] == 1) //第三回合
            {
                JArray api_df_list = (JArray)root.SelectToken("api_data.api_hougeki3.api_df_list");
                JArray api_damage = (JArray)root.SelectToken("api_data.api_hougeki3.api_damage");
                nowhps.UpdateHouGeKiHP(api_df_list, api_damage);
            }

            //是否有闭幕雷击
            int raigeki = houraiFlag[3];
            if (raigeki == 1)
            {
                List<int> api_fdam = root.SelectToken("api_data.api_raigeki.api_fdam").ToObject<List<int>>();
                List<int> api_edam = root.SelectToken("api_data.api_raigeki.api_edam").ToObject<List<int>>();
                nowhps.UpdateSelfHP(api_fdam);
                nowhps.UpdateEnemyHP(api_edam);
            }





        }






    }
}
