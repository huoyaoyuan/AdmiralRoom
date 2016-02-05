using System;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class MaterialLog : ILog
    {
        [Log]
        public DateTime DateTime { get; set; }
        public DateTime LocalDateTime => DateTime.ToLocalTime();
        [Log]
        public DiffNumber Fuel { get; set; }
        [Log]
        public DiffNumber Bull { get; set; }
        [Log]
        public DiffNumber Steel { get; set; }
        [Log]
        public DiffNumber Bauxite { get; set; }
        [Log]
        public DiffNumber InstantBuild { get; set; }
        [Log]
        public DiffNumber InstantRepair { get; set; }
        [Log]
        public DiffNumber Development { get; set; }
        [Log]
        public DiffNumber Improvement { get; set; }
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
