using System;
using System.Collections.Generic;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.Logger;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    public class BattleCatalogWorker
    {
        public class BattleCatalogViewModel
        {
            public string Title { get; set; }
            public int? Total { get; set; }
            public int Win => S + A + B;
            public int S { get; set; }
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
            public int D { get; set; }
            public List<BattleCatalogViewModel> Items { get; } = new List<BattleCatalogViewModel>();
        }
        public BattleCatalogViewModel[] Items { get; }
        public BattleCatalogWorker()
        {
            var today = DateTime.Today;
            //var logs = Loggers.BattleDropLogger.Read().ToList();
            //var groups = new[]
            //{
            //    new { Title = "Today", Logs = logs.Where(x => x.DateTime.ToLocalTime().Date == today) },
            //    new { Title = "Weekly", Logs = logs.Where(x => x.DateTime.ToLocalTime().WeekStart() == today.WeekStart()) },
            //    new { Title = "Monthly", Logs = logs.Where(x => x.DateTime.ToLocalTime().MonthStart() == today.MonthStart()) },
            //    new { Title = "LastWeek", Logs = logs.Where(x => x.DateTime.ToLocalTime().WeekStart().AddDays(7) == today.WeekStart()) },
            //    new { Title = "LastMonth", Logs = logs.Where(x => x.DateTime.ToLocalTime().MonthStart().AddMonths(1) == today.MonthStart()) },
            //    new { Title = "All", Logs = logs.AsEnumerable() }
            //};
            //Items = groups.Select(x =>
            //{
            //    var viewmodel = new BattleCatalogViewModel { Title = x.Title, Total = 0 };
            //    viewmodel.Items.Add(new BattleCatalogViewModel { Title = "BOSS" });
            //    var d = new Dictionary<int, BattleCatalogViewModel>();

            //    return viewmodel;
            //}).ToArray();
            //TODO:还是从战斗记录里读吧
        }
    }
}
