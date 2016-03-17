using System;
using System.Collections.Generic;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Modules.Akashi
{
    class AkashiViewModel : NotificationObject
    {
        #region WeekDay
        private int _weekday = (int)DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(9)).DayOfWeek;
        public int WeekDay
        {
            get { return _weekday; }
            set
            {
                if (_weekday != value)
                {
                    _weekday = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TodayInfo));
                }
            }
        }
        #endregion

        public IReadOnlyList<EnhancementInfo> TodayInfo { get; }
        private IReadOnlyList<EnhancementInfo> AllInfo;
    }
    class EnhancementInfo
    {
        public EquipInfo Equipment { get; set; }
        public SecretaryInfo[] Secretaries { get; set; }
        public GradeInfo[] Ranges { get; set; }
        public int CostItem1 { get; set; }
        public int CostItem2 { get; set; }
        public int CostItem3 { get; set; }
        public int CostItem4 { get; set; }
    }
    class SecretaryInfo
    {
        public string Secratary { get; set; }
        public string RedText { get; set; }
        public int WeekDays { get; set; }
    }
    class GradeInfo
    {
        public int Grade { get; set; }
        public int CostDevelopment { get; set; }
        public int CostImprovement { get; set; }
        public int CostDevelopmentConfirmed { get; set; }
        public int CostImprovementConfirmed { get; set; }
        public EquipInfo CostItem { get; set; }
        public int CostItemCount { get; set; }
        public EquipInfo UpdateTo { get; set; }
    }
}
