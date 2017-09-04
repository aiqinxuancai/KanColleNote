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
            changeHP = new List<string>();
            round = new List<BattleRound>();
        }

        public string name { set; get; }
        public int maxHP { set; get; }
        public int nowHP { set; get; }
        public List<string> changeHP { set; get; }
        public List<BattleRound> round { set; get; }
        public void ChangeHP(int hp)
        {
            nowHP -= hp;
        }
    }
    /// <summary>
    /// 战斗回合
    /// </summary>
    class BattleRound
    {
        /// <summary>
        /// 输出的信息
        /// </summary>
        public string message { set; get; }
        /// <summary>
        /// 最大HP
        /// </summary>
        public int maxHP { set; get; }
        /// <summary>
        /// 被本轮攻击后的HP
        /// </summary>
        public int nowHP { set; get; }
        /// <summary>
        /// 被本轮攻击后的状态
        /// </summary>
        public string state { set; get; }
    }
    public enum Faction {
        /// <summary>
        /// 我方
        /// </summary>
        SELF = 0,
        /// <summary>
        /// 敌方
        /// </summary>
        ENEMY = 1
    };

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

        /// <summary>
        /// 初始化所有数据 
        /// </summary>
        /// <param name="root"></param>
        public BattleHPManager(JObject root)
        {
            Debug.WriteLine("初始化BattleHPManager");

            m_self = new List<BattleUnit>();
            m_enemy = new List<BattleUnit>();
            //当前血量
            List<int> nowhpsList = JsonHelper.SelectTokenIntList(root, "api_data.api_nowhps"); 
            List<int> nowhpsListCombined = JsonHelper.SelectTokenIntList(root, "api_data.api_nowhps_combined");
            //最大血量
            List<int> maxhpsList = JsonHelper.SelectTokenIntList(root, "api_data.api_maxhps");
            List<int> maxhpsListCombined = JsonHelper.SelectTokenIntList(root, "api_data.api_maxhps_combined");
            //敌方ID
            List<int> api_ship_ke = JsonHelper.SelectTokenIntList(root, "api_data.api_ship_ke");
            List<int> api_ship_ke_combined = JsonHelper.SelectTokenIntList(root, "api_data.api_ship_ke_combined");


            List<int> maxhpsSelf;
            List<int> maxhpsEnemy;
            Debug.WriteLine("初始化血量");
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
            Debug.WriteLine("获取船数据");
            InitEnemyShip(api_ship_ke, api_ship_ke_combined);


            InitSelfShip(JsonHelper.SelectTokenInt(root, "api_data.api_dock_id"));

            Debug.WriteLine("创建BattleUnit-m_nowhpsEnemy");
            for (int i = 0; i < m_nowhpsEnemy.Count; i++)
            {
                BattleUnit item = new BattleUnit();
                item.nowHP = m_nowhpsEnemy[i];
                item.maxHP = maxhpsEnemy[i];
                item.name = JsonHelper.SelectTokenString(m_shipEnemy, $"[{i}].api_name", "");
                m_enemy.Add(item);
            }
            Debug.WriteLine("创建BattleUnit-m_nowhpsSelf");
            for (int i = 0; i < m_nowhpsSelf.Count; i++)
            {
                BattleUnit item = new BattleUnit();
                item.nowHP = m_nowhpsSelf[i];
                item.maxHP = maxhpsSelf[i];
                if (i >= 0 && i <= 5)
                {
                    item.name = JsonHelper.SelectTokenString(m_shipShip1, $"$.api_ship_full[{i}].api_ship_data.api_name", "");
                }
                else
                {
                    item.name = JsonHelper.SelectTokenString(m_shipShip2, $"$.api_ship_full[{i - 6}].api_ship_data.api_name", "");
                }


                m_self.Add(item);
            }

            Debug.WriteLine("初始化BattleHPManager完毕");
        }


        public void InitSelfShip(int api_deck_id)
        {
            if (m_nowhpsSelf.Count == 6)
            {
                //api_deck_id
                m_shipShip1 = (JObject)KanPort.GetTeam(api_deck_id);
                Debug.WriteLine(m_shipShip1);
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
            Debug.WriteLine(name);
        }




        /// <summary>
        /// HP变更
        /// </summary>
        /// <param name="faction"></param>
        /// <param name="shipIndexId"></param>
        /// <param name="hp"></param>
        public void HPChangeEvent(Faction faction, int attackerId, int shipIndexId, int hp, string attackTypeName = "")
        {
            //记录一套日志
            if (faction == Faction.ENEMY)
            {
                m_enemy[shipIndexId].ChangeHP(hp); //受伤的是敌方
                var hpShow = $"[{ m_enemy[shipIndexId].nowHP}/{m_enemy[shipIndexId].maxHP}] {GetDamageState(m_enemy[shipIndexId].nowHP, m_enemy[shipIndexId].maxHP)}"; //GetDamageState
                var message = "";
                if (attackerId == -1)
                {
                    if (hp != 0)
                    {
                        message = $"{m_eventName} {m_enemy[shipIndexId].name}({shipIndexId + 1}) -{hp} {hpShow}";
                    }  
                } 
                else
                {
                    message = $"{m_eventName} {attackTypeName} {m_self[attackerId].name}({attackerId + 1}) -> {m_enemy[shipIndexId].name}({shipIndexId + 1}) -{hp} {hpShow}";
                }
                BattleRound round = new BattleRound() {
                    message = message,
                    maxHP = m_enemy[shipIndexId].maxHP,
                    nowHP = m_enemy[shipIndexId].nowHP,
                    state = GetDamageState(m_enemy[shipIndexId].nowHP, m_enemy[shipIndexId].maxHP)
                };
                if (message != string.Empty)
                {
                    Debug.WriteLine(message);
                    m_enemy[shipIndexId].changeHP.Add(message);
                    m_enemy[shipIndexId].round.Add(round);
                }
                 
            }
            else if (faction == Faction.SELF) //受伤的是我方
            {
                m_self[shipIndexId].ChangeHP(hp);

                var hpShow = $"[{m_self[shipIndexId].nowHP}/{m_self[shipIndexId].maxHP}] {GetDamageState(m_self[shipIndexId].nowHP, m_self[shipIndexId].maxHP)}";
                var message = "";
                if (attackerId == -1)
                {
                    if (hp != 0)
                    {
                        message = $"{m_eventName} {m_self[shipIndexId].name}({shipIndexId + 1}) -{hp} {hpShow}";
                    }
                }
                else
                {
                    message = $"{m_eventName} {attackTypeName} {m_enemy[attackerId].name}({attackerId + 1}) -> {m_self[shipIndexId].name}({shipIndexId + 1}) -{hp} {hpShow}";
                }

                BattleRound round = new BattleRound()
                {
                    message = message,
                    maxHP = m_self[shipIndexId].maxHP,
                    nowHP = m_self[shipIndexId].nowHP,
                    state = GetDamageState(m_self[shipIndexId].nowHP, m_self[shipIndexId].maxHP)
                };

                if (message != string.Empty)
                {
                    Debug.WriteLine(message);
                    m_self[shipIndexId].changeHP.Add(message);
                    m_self[shipIndexId].round.Add(round);
                }
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
                HPChangeEvent(Faction.SELF, -1, i, updateHps[i]);
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
                HPChangeEvent(Faction.SELF, -1, i, updateHps[i - 6]);
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
                HPChangeEvent(Faction.ENEMY, -1, i, updateHps[i]);
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
                HPChangeEvent(Faction.ENEMY, -1, i, updateHps[i - 6]);
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
        /// <param name="targets">谁攻击了谁</param>
        /// <param name="damages">造成伤害</param>
        /// <returns></returns>
        public void UpdateHouGeKiHP(JArray targets, JArray damages, JArray attackers, JArray attackTypes, BattleTime battleTime, List<int> eflag = null, int selfTeamId = 1, int enemyTeamId = 1) //api_hougeki1
        {
            if (targets == null || damages == null) return;

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                int attackerId = attackers[i].Value<int>(); //攻击者
                int attackType = 0;
                string attackTypeName = "";
                if (attackTypes != null)
                {
                    attackType = attackTypes[i].Value<int>(); //攻击类型
                    attackTypeName = GetAttackTypeName(battleTime, attackType);
                }
                //子数组 比如二连则是俩 普通攻击则是一个
                if (target.Type == JTokenType.Array)
                {
                    JArray targetArray = (JArray)target;
                    for (int x = 0; x < targetArray.Count; x++) //目标子数组 连击？ CI？
                    {
                        int targetId = targetArray[x].Value<int>(); //攻击到谁
                        int damageHp = damages[i][x].Value<int>();   //减少多少血量

                        if (eflag != null) //联合舰队 有eflag的处理方式
                        {
                            //联合舰队战斗
                            if (eflag[i] == 0) //由我方发起的攻击 减少对面的血量
                            {
                                m_nowhpsEnemy[targetId - 1] -= damageHp;
                                HPChangeEvent(Faction.ENEMY, attackerId - 1, targetId - 1, damageHp, attackTypeName); //传递的id为index 从0开始
                            }
                            else if (eflag[i] == 1) //由敌方发起
                            {
                                m_nowhpsSelf[targetId - 1] -= damageHp;
                                HPChangeEvent(Faction.SELF, attackerId - 1, targetId - 1 , damageHp, attackTypeName);
                            }
                        }
                        else //普通战斗|夜战
                        {

                            if (targetId >= 1 && targetId <= 6)
                            {
                                //我方第一队伍
                                int id = targetId - 1 + (selfTeamId - 1) * 6;
                                int newAttackerId = -1;
                                //计算正确的攻击者ID
                                if (attackerId >= 1 && attackerId <= 6)
                                {
                                    newAttackerId = attackerId - 1 + (enemyTeamId - 1) * 6; 
                                }
                                else
                                {
                                    newAttackerId = attackerId - 7 + (enemyTeamId - 1) * 6;
                                }

                                m_nowhpsSelf[id] -= damageHp;
                                HPChangeEvent(Faction.SELF, newAttackerId, id, damageHp, attackTypeName);
                            }
                            if (targetId >= 7 && targetId <= 12)
                            {
                                //敌方第一队伍
                                int id = targetId - 7 + (enemyTeamId - 1) * 6;
                                int newAttackerId = -1;
                                //计算正确的攻击者ID
                                if (attackerId >= 1 && attackerId <= 6)
                                {
                                    newAttackerId = attackerId - 1 + (enemyTeamId - 1) * 6;
                                }
                                else
                                {
                                    newAttackerId = attackerId - 7 + (enemyTeamId - 1) * 6;
                                }
                                m_nowhpsEnemy[id] -= damageHp;
                                HPChangeEvent(Faction.ENEMY, newAttackerId, id, damageHp, attackTypeName);
                            }
                        }
                        //m_nowhps[targetId] -= damageHp;
                    }
                }
            }
        }


        public string GetAttackTypeName(BattleTime battleTime, int id)
        {
            if (battleTime == BattleTime.Day)
            {
                // 0=普通 1=？？ 2=连击 3=主炮副炮 4=主炮电探 5=主炮撤甲 6=主炮主炮
                switch (id)
                {
                    case 0:
                        return "通常攻击";
                    case 1:
                        return "？？攻击";
                    case 2:
                        return "连击";
                    case 3:
                        return "着弹观测（主+副）";
                    case 4:
                        return "着弹观测（主+电）";
                    case 5:
                        return "着弹观测（主+弹）";
                    case 6:
                        return "着弹观测（主+主）";
                }
            }
            else
            {
                switch (id)
                {
                    case 0:
                        return "通常攻击";
                    case 1:
                        return "连击";
                    case 2:
                        return "CI（主炮+鱼雷）";
                    case 3:
                        return "CI（鱼雷+鱼雷）";
                    case 4:
                        return "CI（主炮+副炮）";
                    case 5:
                        return "CI（主炮+主炮）";
                }
            }
            return "";
        }

        public string GetDamageState(int nowHP, int maxHP)
        {
            double hprate = (double)nowHP / maxHP;
            if (hprate <= 0.0)
                return "击沉"; //或脱离？
            else if (hprate <= 0.25)
                return "大破";
            else if (hprate <= 0.5)
                return "中破";
            else if (hprate <= 0.75)
                return "小破";
            else if (hprate < 1.0)
                return ""; //擦伤
            else
                return ""; //无伤
        }
    }

}
