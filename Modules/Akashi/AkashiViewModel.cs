using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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

        public IReadOnlyList<EnhancementInfo> TodayInfo => AllInfo.Select(x => x.CopyOfWeekDay(WeekDay)).Where(x => x.Secretaries.Any()).ToArray();
        private readonly IReadOnlyList<EnhancementInfo> AllInfo
            = (IReadOnlyList<EnhancementInfo>)Application.LoadComponent(new Uri("Akashi;component/AkashiData.xaml", UriKind.Relative));
    }
    class EnhancementInfo
    {
        public int EquipId { get; set; }
        public EquipInfo Equipment => Staff.Current.MasterData.EquipInfo?[EquipId];
        public List<SecretaryInfo> Secretaries { get; set; } = new List<SecretaryInfo>();
        public List<RangeInfo> Ranges { get; set; } = new List<RangeInfo>();
        public int CostItem1 { get; private set; }
        public int CostItem2 { get; private set; }
        public int CostItem3 { get; private set; }
        public int CostItem4 { get; private set; }
        public string Cost
        {
            set
            {
                var costs = value.Split(',');
                CostItem1 = int.Parse(costs[0]);
                CostItem2 = int.Parse(costs[1]);
                CostItem3 = int.Parse(costs[2]);
                CostItem4 = int.Parse(costs[3]);
            }
        }
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
        public int Range { get; set; }
        public int CostDevelopment { get; private set; }
        public int CostImprovement { get; private set; }
        public int CostDevelopmentConfirmed { get; private set; }
        public int CostImprovementConfirmed { get; private set; }
        public string Cost
        {
            set
            {
                var costs = value.Split(',');
                CostDevelopment = int.Parse(costs[0]);
                CostDevelopmentConfirmed = int.Parse(costs[1]);
                CostImprovement = int.Parse(costs[2]);
                CostImprovementConfirmed = int.Parse(costs[3]);
            }
        }
        public int CostItemId { get; private set; }
        public EquipInfo CostItem => Staff.Current.MasterData.EquipInfo?[CostItemId];
        public int CostItemCount { get; private set; }
        public string CostEquip
        {
            set
            {
                var cost = value.Split(',');
                CostItemId = int.Parse(cost[0]);
                CostItemCount = int.Parse(cost[1]);
            }
        }
        public int UpgradeToId { get; set; }
        public EquipInfo UpgradeTo => Staff.Current.MasterData.EquipInfo?[UpgradeToId];
    }
}
