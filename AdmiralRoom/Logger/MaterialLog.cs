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
        public int TakeValue(int id)
        {
            switch (id)
            {
                case 1:
                    return Fuel;
                case 2:
                    return Bull;
                case 3:
                    return Steel;
                case 4:
                    return Bauxite;
                case 5:
                    return InstantBuild;
                case 6:
                    return InstantRepair;
                case 7:
                    return Development;
                case 8:
                    return Improvement;
                default:
                    throw new ArgumentException(nameof(id));
            }
        }
    }
    public class MaterialLogger : CsvLogger<MaterialLog>
    {
        public MaterialLogger(string filename) : base(filename) { }
        private DateTimeOffset lastlog;
        private readonly TimeSpan loginterval = TimeSpan.FromMinutes(10);
        public bool? ForceLog { private get; set; }
        public void TryLog(Officer.Material material)
        {
            var now = DateTimeOffset.Now;
            if (ForceLog == false)
            {
                ForceLog = null;
                return;
            }
            if (ForceLog == true || now - lastlog >= loginterval)
            {
                Log(MaterialLog.Now(material));
                lastlog = now;
                ForceLog = null;
            }
        }
    }
}
