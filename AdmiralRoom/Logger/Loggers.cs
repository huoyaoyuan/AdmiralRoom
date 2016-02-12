namespace Huoyaoyuan.AdmiralRoom.Logger
{
    static class Loggers
    {
        public static void Initialize()
        {
            System.IO.Directory.CreateDirectory("logs");
            CreateItemLogger = new CsvLogger<CreateItemLog>(@"logs\createitem.csv") { TitleKey = "Logger_CreateItem" };
            CreateShipLogger = new CsvLogger<CreateShipLog>(@"logs\createship.csv") { TitleKey = "Logger_CreateShip" };
            MissionLogger = new CsvLogger<MissionLog>(@"logs\mission.csv") { TitleKey = "Expedition" };
            BattleDropLogger = new CsvLogger<BattleDropLog>(@"logs\drop.csv") { TitleKey = "Logger_Drop" };
            MaterialLogger = new MaterialLogger(@"logs\material.csv");
        }
        public static CsvLogger<CreateItemLog> CreateItemLogger { get; private set; }
        public static CsvLogger<CreateShipLog> CreateShipLogger { get; private set; }
        public static CsvLogger<MissionLog> MissionLogger { get; private set; }
        public static CsvLogger<BattleDropLog> BattleDropLogger { get; private set; }
        public static MaterialLogger MaterialLogger { get; private set; }
    }
}
