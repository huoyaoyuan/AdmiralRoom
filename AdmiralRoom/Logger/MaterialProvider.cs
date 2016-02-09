using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    sealed class MaterialProvider
    {
        public IEnumerable Durations { get; }
        public IReadOnlyList<MaterialLog> All { get; }
        public IReadOnlyList<MaterialLog> AsDays { get; }
        public DateTime Now { get; } = DateTime.UtcNow;
        public DateTime From { get; set; }
        public MaterialProvider(MaterialLogger logger)
        {
            All = logger.Read().ToArray();
            AsDays = MakeDiff(EachDay(All)).Reverse().ToArray();
            DateTime firsttime = Now;
            if (All.Count > 0) firsttime = All[0].DateTime;
            Durations = new[]
            {
                new { TitleKey = "Resources.ChartTimeRange_Days1", From = Now.AddDays(-1) },
                new { TitleKey = "Resources.ChartTimeRange_Weeks1", From = Now.AddDays(-7) },
                new { TitleKey = "Resources.ChartTimeRange_Weeks2", From = Now.AddDays(-14) },
                new { TitleKey = "Resources.ChartTimeRange_Months1", From = Now.AddMonths(-1) },
                new { TitleKey = "Resources.ChartTimeRange_Months2", From = Now.AddMonths(-2) },
                new { TitleKey = "Resources.ChartTimeRange_Months3", From = Now.AddMonths(-3) },
                new { TitleKey = "Resources.ChartTimeRange_Months6", From = Now.AddMonths(-6) },
                new { TitleKey = "Resources.ChartTimeRange_Years1", From = Now.AddYears(-1) },
                new { TitleKey = "Resources.ChartTimeRange_All", From = firsttime }
            };
            From = Now.AddDays(-14);
        }
        public static IEnumerable<MaterialLog> EachDay(IEnumerable<MaterialLog> source)
        {
            MaterialLog last = null;
            foreach (var log in source)
            {
                if (last != null)
                {
                    if (last.DateTime.ToLocalTime().Date != log.DateTime.ToLocalTime().Date)
                        yield return last;
                }
                last = log;
            }
            if (last != null) yield return last;
        }
        public static IEnumerable<MaterialLog> MakeDiff(IEnumerable<MaterialLog> source)
        {
            MaterialLog last = null;
            foreach (var log in source)
            {
                if (last != null)
                {
                    log.Fuel -= last.Fuel;
                    log.Bull -= last.Bull;
                    log.Steel -= last.Steel;
                    log.Bauxite -= last.Bauxite;
                    log.InstantBuild -= last.InstantBuild;
                    log.InstantRepair -= last.InstantRepair;
                    log.Development -= last.Development;
                    log.Improvement -= last.Improvement;
                }
                last = log;
                yield return log;
            }
        }
    }
}
