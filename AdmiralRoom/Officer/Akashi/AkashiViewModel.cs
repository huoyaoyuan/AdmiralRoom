using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Officer.Akashi
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

        public IReadOnlyList<EnhancementInfo> TodayInfo => AllInfo.Select(x => x.CopyOfWeekDay(WeekDay)).Where(x => x.Secretaries.Any()).ToArray();
        private readonly IReadOnlyList<EnhancementInfo> AllInfo
            = (IReadOnlyList<EnhancementInfo>)Application.LoadComponent(new Uri("AdmiralRoom;component/Officer/Akashi/AkashiData.xaml", UriKind.Relative));
    }
    class EnhancementInfo
    {
        public int EquipId { get; set; }
        public EquipInfo Equipment => Staff.Current.MasterData.EquipInfo?[EquipId];
        public List<SecretaryInfo> Secretaries { get; set; } = new List<SecretaryInfo>();
        public List<RangeInfo> Ranges { get; set; } = new List<RangeInfo>();
        public int CostItem1 { get; set; }
        public int CostItem2 { get; set; }
        public int CostItem3 { get; set; }
        public int CostItem4 { get; set; }
        public EnhancementInfo CopyOfWeekDay(int weekday) => new EnhancementInfo
        {
            EquipId = EquipId,
            Secretaries = Secretaries.Where(x => (x.WeekDays & (1 << weekday)) != 0).ToList(),
            Ranges = Ranges,
            CostItem1 = CostItem1,
            CostItem2 = CostItem2,
            CostItem3 = CostItem3,
            CostItem4 = CostItem4
        };
    }
    struct SecretaryInfo
    {
        public string Secratary { get; set; }
        public string RedText { get; set; }
        public int WeekDays { get; set; }
    }
    struct RangeInfo
    {
        public int CostDevelopment { get; set; }
        public int CostImprovement { get; set; }
        public int CostDevelopmentConfirmed { get; set; }
        public int CostImprovementConfirmed { get; set; }
        public int CostItemId { get; set; }
        public EquipInfo CostItem => Staff.Current.MasterData.EquipInfo?[CostItemId];
        public int CostItemCount { get; set; }
        public int UpgradeToId { get; set; }
        public EquipInfo UpgradeTo => Staff.Current.MasterData.EquipInfo?[UpgradeToId];
        public int UpgradeStar { get; set; }
        public int UseItemId { get; set; }
        public int UseItemCount { get; set; }
        public UseItem UseItem => Staff.Current.MasterData.UseItems?[UseItemId];
    }
}
