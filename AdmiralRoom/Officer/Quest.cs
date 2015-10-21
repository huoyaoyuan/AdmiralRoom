using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Quest : GameObject<api_quest>, IIdentifiable
    {
        public override int Id => rawdata.api_no;
        public QuestType Type => (QuestType)rawdata.api_category;
        public QuestPeriod Period => (QuestPeriod)rawdata.api_type;
        public bool IsDaily => Period == QuestPeriod.Daily || Period == QuestPeriod.Day037 || Period == QuestPeriod.Day28;
        public QuestState State => (QuestState)rawdata.api_state;
        public string Title => rawdata.api_title ?? "（未知任务）";
        public string Detail => rawdata.api_detail;
        public int[] GetMaterial => rawdata.api_get_material;
        public QuestBonus Bonus => (QuestBonus)rawdata.api_bonus_flag;
        public QuestProgress Progress => (QuestProgress)rawdata.api_progress_flag;
        /// <summary>
        /// 机种转换不能完成？
        /// </summary>
        public bool Invalid => rawdata.api_invalid_flag != 0;
        public Quest() { }
        public Quest(api_quest api) : base(api) { }
    }
    public enum QuestType { Unknown = 0, Fleet = 1, Battle = 2, Exercise = 3, Expedition = 4, Charge = 5, Shipyard = 6, Powerup = 7, Other = 8 }
    public enum QuestPeriod { Once = 1, Daily = 2, Weekly = 3, Day037 = 4, Day28 = 5, Monthly = 6 }
    public enum QuestState { None = 1, InProgress = 2, Complete = 3 }
    public enum QuestBonus { None = 0, Normal = 1, Ship = 2 }
    public enum QuestProgress { None = 0, Percent50 = 1, Percent80 = 2 }
}
