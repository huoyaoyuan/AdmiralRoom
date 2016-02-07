using System.Collections.Generic;
using System.Linq;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    sealed class MaterialProvider
    {
        public IReadOnlyList<MaterialLog> All { get; }
        public IReadOnlyList<MaterialLog> AsDays { get; }
        public MaterialProvider(MaterialLogger logger)
        {
            All = logger.Read().ToArray();
            AsDays = MakeDiff(EachDay(All)).Reverse().ToArray();
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
