using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote.Model
{
    class NotificationType
    {

        public const string kKanMasterIdChange = "KanMasterIdChange";
        /// <summary>
        /// 玩家昵称变更后 所有数据重新加载完毕后
        /// </summary>
        public const string kKanMasterIdChangeAfter = "KanMasterIdChangeAfter";
        
        public const string kConfigUpdate = "ConfigUpdate";
        public const string kSourceUpdate = "SourceUpdate";
        public const string kMissionUpdate = "MissionUpdate";
        public const string kBattleResultUpdate = "BattleResultUpdate";
        public const string kBattleResultBindingUpdate = "BattleResultBindingUpdate";

        /// <summary>
        /// 先知要刷新
        /// </summary>
        public const string kBattleProphetUpdate = "BattleProphetUpdate";
    }
}
