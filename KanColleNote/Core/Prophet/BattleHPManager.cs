using KanColleNote.Base;
using KanColleNote.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core.Prophet
{
    /// <summary>
    /// 战斗单位
    /// </summary>
    class BattleUnit
    {
        public BattleUnit()
        {
        }
        public string name { set; get; }
        public int maxHP { set; get; }
        public int nowHP { set; get; }
        public List<string> changeHP { set; get; }

        public void ChangeHP(int hp)
        {
            nowHP -= hp;
        }
    }

    public enum Faction { SELF = 0, ENEMY = 1 };

    class BattleHPManager
    {
        public List<BattleUnit> m_self;
        public List<BattleUnit> m_enemy;

        public List<int> m_nowhpsSelf;
        public List<int> m_nowhpsEnemy;
        
        public JArray m_shipEnemy; //敌方的船具体名字等 按照顺序6或12只 api_name

        public JObject m_shipShip1; //我方的队伍1数据
        public JObject m_shipShip2; //我方的队伍2数据

        public string m_eventName; 

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


        public BattleHPManager(JObject root)
        {
            //当前血量
            List<int> nowhpsList = JsonHelper.SelectTokenIntList(root, "api_data.api_nowhps"); 
            List<int> nowhpsListCombined = JsonHelper.SelectTokenIntList(root, "api_data.api_nowhps_combined");
            //最大血量
            List<int> maxhpsList = JsonHelper.SelectTokenIntList(root, "api_data.api_maxhps");
            List<int> maxhpsListCombined = JsonHelper.SelectTokenIntList(root, "api_data.api_maxhps_combined");
            //敌方ID
            List<int> api_ship_ke = JsonHelper.SelectTokenIntList(root, "api_data.api_ship_ke");
            List<int> api_ship_ke_combined = JsonHelper.SelectTokenIntList(root, "api_data.api_ship_ke_combined");
;

            List<int> maxhpsSelf;
            List<int> maxhpsEnemy;

            m_shipEnemy = new JArray();
            m_nowhpsSelf = nowhpsList.GetRange(1, 6);
            m_nowhpsEnemy = nowhpsList.GetRange(7, 6);
            maxhpsSelf = maxhpsList.GetRange(1, 6);
            maxhpsEnemy = maxhpsList.GetRange(7, 6);
            if (nowhpsListCombined != null)
            {
                m_nowhpsSelf.AddRange(nowhpsListCombined.GetRange(1, 6));
                maxhpsSelf.AddRange(maxhpsListCombined.GetRange(1, 6));

                if (nowhpsListCombined.Count == 13)
                {
                    m_nowhpsEnemy.AddRange(nowhpsListCombined.GetRange(7, 6));
                    maxhpsEnemy.AddRange(maxhpsListCombined.GetRange(7, 6));
                }
            }

            InitEnemyShip(api_ship_ke, api_ship_ke_combined);
            InitSelfShip(JsonHelper.SelectTokenInt(root, "api_data.api_deck_id"));



            for (int i = 0; i < m_nowhpsEnemy.Count; i++)
            {
                BattleUnit item = new BattleUnit();
                item.nowHP = m_nowhpsEnemy[i];
                item.maxHP = maxhpsEnemy[i];
                item.name = JsonHelper.SelectTokenString(m_shipEnemy, $"[{i}].api_name", "");
                m_enemy.Add(item);
            }

            for (int i = 0; i < m_nowhpsSelf.Count; i++)
            {
                BattleUnit item = new BattleUnit();
                item.nowHP = m_nowhpsSelf[i];
                item.maxHP = maxhpsSelf[i];
                if (i >= 0 && i <= 5)
                {
                    item.name = JsonHelper.SelectTokenString(m_shipShip1, $"$.api_ship_full[{i}].api_name", "");
                }
                else
                {
                    item.name = JsonHelper.SelectTokenString(m_shipShip2, $"$.api_ship_full[{i - 6}].api_name", "");
                }


                m_self.Add(item);
            }
        }


        public void InitSelfShip(int api_deck_id)
        {
            if (m_nowhpsSelf.Count == 6)
            {
                //api_deck_id
                m_shipShip1 = (JObject)KanPort.GetTeam(api_deck_id);
            }
            if (m_nowhpsSelf.Count == 12)
            {
                //取舰队1-2
                m_shipShip1 = (JObject)KanPort.GetTeam(1);
                m_shipShip2 = (JObject)KanPort.GetTeam(2);
            }
        }


        public void SetEventName(string name)
        {
            m_eventName = name;
        }

        public void HPChangeEvent(Faction faction, int shipIndexId, int hp)
        {
            
            //记录一套日志
            if (faction == Faction.ENEMY)
            {
                string name = "";
                if (m_shipEnemy != null)
                {
                    name = JsonHelper.SelectTokenString(m_shipEnemy, $"[{shipIndexId}].api_name");
                }
                Debug.WriteLine(faction.ToString() + $" {name}({shipIndexId}) {hp}");
            }
            else if (faction == Faction.SELF)
            {
                string name = "";
                if (shipIndexId >= 1 && shipIndexId <=6)
                {
                    if (m_shipShip1 != null)
                    {
                        //name = m_shipShip1["api_ship_data"][shipIndexId]["api_name"].Value<string>();
                        name = JsonHelper.SelectTokenString(m_shipShip1, $"$.api_ship_data[{shipIndexId}].api_name");
                    }
                }
                else
                {
                    if (m_shipShip2 != null)
                    {
                        //name = m_shipShip2["api_ship_data"][shipIndexId]["api_name"].Value<string>();
                        name = JsonHelper.SelectTokenString(m_shipShip2, $"$.api_ship_data[{shipIndexId}].api_name");
                    }
                }
                Debug.WriteLine(faction.ToString() + $" {name}({shipIndexId}) {hp}");
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
                HPChangeEvent(Faction.SELF, i, updateHps[i]);
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
                HPChangeEvent(Faction.SELF, i, updateHps[i]);
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
                HPChangeEvent(Faction.ENEMY, i, updateHps[i]);
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
                HPChangeEvent(Faction.ENEMY, i, updateHps[i]);
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
            if (df == null || damage == null) return;
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
                                HPChangeEvent(Faction.ENEMY, targetId, damageHp);
                            }
                            else if (eflag[i] == 1) //由敌方发起
                            {
                                m_nowhpsSelf[targetId - 1] -= damageHp;
                                HPChangeEvent(Faction.SELF, targetId, damageHp);
                            }
                        }
                        else
                        {
                            //普通战斗
                            if (targetId >= 1 && targetId <= 6)
                            {
                                //我方第一队伍
                                int id = targetId - 1 + (selfTeamId - 1) * 6;
                                m_nowhpsSelf[id] -= damageHp;
                                HPChangeEvent(Faction.SELF, id, damageHp);
                            }
                            if (targetId >= 7 && targetId <= 12)
                            {
                                //敌方第一队伍
                                int id = targetId - 7 + (enemyTeamId - 1) * 6;
                                m_nowhpsEnemy[id] -= damageHp;
                                HPChangeEvent(Faction.ENEMY, id, damageHp);
                            }
                        }
                        //m_nowhps[targetId] -= damageHp;
                    }
                }
            }
        }
    }

}
