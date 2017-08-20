using KanColleNote.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core.Prophet
{
    class BattleHPManager
    {
        public List<int> m_nowhpsSelf;
        public List<int> m_nowhpsEnemy;

        public JArray m_shipEnemy; //敌方的船具体名字等 按照顺序6或12只
        public JArray m_shipShip; //我方的船具体名字等 按照顺序6或12只



        public BattleHPManager(List<int> nowhps, List<int> nowhps_combined = null)
        {
            m_shipEnemy = new JArray();
            m_nowhpsSelf = nowhps.GetRange(1, 6);
            m_nowhpsEnemy = nowhps.GetRange(7, 6);
            if (nowhps_combined != null)
            {
                m_nowhpsSelf.AddRange(nowhps_combined.GetRange(1, 6));
                if (nowhps_combined.Count == 13)
                {
                    m_nowhpsEnemy.AddRange(nowhps_combined.GetRange(7, 6));
                }
            }
        }

        public void InitSelfShip(int api_deck_id)
        {
            if (m_nowhpsSelf.Count == 6)
            {
                //api_deck_id
                KanPort.GetTeam(api_deck_id);
            }
            if (m_nowhpsSelf.Count == 12)
            {
                //取舰队1-2
                KanPort.GetTeam(1);
                KanPort.GetTeam(2);
            }
        }


        /// <summary>
        /// 初始化敌方船的数据
        /// </summary>
        /// <param name="api_ship_ke"></param>
        /// <param name="api_ship_ke_combined"></param>
        public void InitEnemyShip(List<int> api_ship_ke, List<int> api_ship_ke_combined)
        {
            if (api_ship_ke != null)
            {
                for (int i = 1; i < api_ship_ke.Count; i++)
                {

                    var shipData = KanDataCore.GetAnyWithId(KanDataType.SHIP, api_ship_ke[i]);
                    m_shipEnemy.Add(shipData);
                }
            }
            if (api_ship_ke_combined != null)
            {
                for (int i = 1; i < api_ship_ke_combined.Count; i++)
                {
                    var shipData = KanDataCore.GetAnyWithId(KanDataType.SHIP, api_ship_ke_combined[i]);
                    m_shipEnemy.Add(shipData);
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
            if (updateHps == null)
            {
                return;
            }
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
            if (updateHps == null)
            {
                return;
            }
            updateHps = HPListCheck(updateHps);
            for (int i = 6; i < m_nowhpsSelf.Count; i++)
            {
                m_nowhpsSelf[i] -= updateHps[i - 6];
                if (updateHps[i - 6] != 0)
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
            if (updateHps == null)
            {
                return;
            }
            updateHps = HPListCheck(updateHps);
            for (int i = 0; i < updateHps.Count; i++)
            {
                m_nowhpsEnemy[i] -= updateHps[i];
                if (updateHps[i] != 0)
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
            if (updateHps == null)
            {
                return;
            }
            updateHps = HPListCheck(updateHps);
            for (int i = 6; i < m_nowhpsEnemy.Count; i++)
            {
                m_nowhpsEnemy[i] -= updateHps[i - 6];
                if (updateHps[i - 6] != 0)
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
                            if (eflag[i] == 0) //由我方发起的攻击 减少对面的血量
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

}
