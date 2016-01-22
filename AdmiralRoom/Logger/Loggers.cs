namespace Huoyaoyuan.AdmiralRoom.Logger
{
    static class Loggers
    {
        public static void Initialize()
        {
            CreateItemLogger = new CsvLogger<CreateItemLog>(@"logs\createitem.csv") { TitleKey = "Logger_CreateItem" };
            CreateShipLogger = new CsvLogger<CreateShipLog>(@"logs\createship.csv") { TitleKey = "Logger_CreateShip" };
            MissionLogger = new CsvLogger<MissionLog>(@"logs\mission.csv") { TitleKey = "Expedition" };
        }
        public static CsvLogger<CreateItemLog> CreateItemLogger { get; private set; }
        public static CsvLogger<CreateShipLog> CreateShipLogger { get; private set; }
        public static CsvLogger<MissionLog> MissionLogger { get; private set; }
    }
}
