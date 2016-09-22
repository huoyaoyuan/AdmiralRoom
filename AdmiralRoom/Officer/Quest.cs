using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Huoyaoyuan.AdmiralRoom.API;
using Meowtrix;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Quest : GameObject<api_quest>, IIdentifiable<int>
    {
        public override int Id => rawdata.api_no;
        public QuestType Type => (QuestType)rawdata.api_category;
        public QuestPeriod Period => (QuestPeriod)rawdata.api_type;
        public QuestState State => (QuestState)rawdata.api_state;
        public string Title => rawdata.api_title ?? "（未知任务）";
        public string Detail => rawdata.api_detail.Replace("<br>", "");
        public int[] GetMaterial => rawdata.api_get_material;
        public QuestBonus Bonus => (QuestBonus)rawdata.api_bonus_flag;
        public QuestProgress Progress => (QuestProgress)rawdata.api_progress_flag;
        /// <summary>
        /// 机种转换不能完成？
        /// </summary>
        public bool Invalid => rawdata.api_invalid_flag != 0;
        public Quest(api_quest api) : base(api) { }
        private ImageSource _icon;
        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    try
                    {
                        _icon = new BitmapImage(new Uri($"pack://application:,,,/AdmiralRoom;component/Images/Quest/{(int)Type}.png", UriKind.Absolute));
                    }
                    catch
                    {
                        _icon = new BitmapImage(new Uri("pack://application:,,,/AdmiralRoom;component/Images/Quest/9.png", UriKind.Absolute));
                    }
                }
                return _icon;
            }
        }
        public QuestInfo Counter { get; set; }
        public bool HasCounter => Counter != null;
        protected override void UpdateProp()
        {
            Counter = QuestManager.KnownQuests.Known[Id];
            if (Progress == QuestProgress.Percent50) Counter?.Set50();
            else if (Progress == QuestProgress.Percent80) Counter?.Set80();
            if (State == QuestState.Complete) Counter?.Set100();
            Counter?.SetIsTook(State == QuestState.InProgress);
        }
    }
    public enum QuestType { Unknown = 0, Organization = 1, Sortie = 2, Exercise = 3, Expedition = 4, Charge = 5, Shipyard = 6, Powerup = 7, Sortie2 = 8, Other = 9 }
    public enum QuestPeriod { Daily = 1, Weekly = 2, Monthly = 3, Once = 4, Other = 5 }
    public enum QuestState { None = 1, InProgress = 2, Complete = 3 }
    public enum QuestBonus { None = 0, Normal = 1, Ship = 2 }
    public enum QuestProgress { None = 0, Percent50 = 1, Percent80 = 2 }
}
