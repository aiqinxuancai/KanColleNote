using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Core
{
    class KanBattle
    {
        public static JObject m_lastStart;
        public static JObject m_lastBattle;
        public static JObject m_lastBattleResult;


        public static bool SetStartData(string json)
        {
            try
            {
                m_lastStart = JObject.Parse(json);
                Debug.WriteLine("开始一场战斗");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 将Next的数据合并写入m_lastStart
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool SetNextData(string json)
        {
            try
            {
                var next = JObject.Parse(json);

                if (m_lastStart != null)
                {
                    m_lastStart.Merge(next);
                    Debug.WriteLine("合并战斗当前点数据");
                    //合并点数据
                }

                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }


        /// <summary>
        /// 计算伤害 得出剧透结果
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool SetBattleData(string json)
        {
            try
            {
                if (m_lastStart == null)
                {
                    return false;
                }


                m_lastBattle = JObject.Parse(json);
                Debug.WriteLine("战斗数据");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 记录掉落等等
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool SetBattleResultData(string json)
        {
            try
            {
                if (m_lastStart == null)
                {
                    return false;
                }

                m_lastBattleResult = JObject.Parse(json);
                Debug.WriteLine("战斗结果");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }


    }
}
