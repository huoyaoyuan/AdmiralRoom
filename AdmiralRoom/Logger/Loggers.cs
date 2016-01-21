namespace Huoyaoyuan.AdmiralRoom.Logger
{
    static class Loggers
    {
        public static void Initialize()
        {
            CreateItemLogger = new CsvLogger<CreateItemLog>(@"logs\createitem.csv") { TitleKey = "Logger_CreateItem" };
        }
        public static CsvLogger<CreateItemLog> CreateItemLogger { get; private set; }
    }
}
