using System;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class MaterialLog : ILog
    {
        [Log]
        public DateTime DateTime { get; set; }
        [Log]
        public int Fuel { get; set; }
        [Log]
        public int Bull { get; set; }
        [Log]
        public int Steel { get; set; }
        [Log]
        public int Bauxite { get; set; }
        [Log]
        public int InstantBuild { get; set; }
        [Log]
        public int InstantRepair { get; set; }
        [Log]
        public int Development { get; set; }
        [Log]
        public int Improvement { get; set; }
        public static MaterialLog Now(Officer.Material material)
            => new MaterialLog
            {
                DateTime = DateTime.UtcNow,
                Fuel = material.Fuel,
                Bull = material.Bull,
                Steel = material.Steel,
                Bauxite = material.Bauxite,
                InstantBuild = material.InstantBuild,
                InstantRepair = material.InstantRepair,
                Development = material.DevelopmentKit,
                Improvement = material.ImprovementKit
            };
    }
    public class MaterialLogger : CsvLogger<MaterialLog>
    {
        public MaterialLogger(string filename) : base(filename) { }
        private DateTimeOffset lastlog;
        private readonly TimeSpan loginterval = TimeSpan.FromMinutes(10);
        public void TryLog(Officer.Material material)
        {
            var now = DateTimeOffset.Now;
            if (now - lastlog < loginterval) return;
            Log(MaterialLog.Now(material));
            lastlog = now;
        }
    }
}
