namespace Huoyaoyuan.AdmiralRoom.Logger
{
    static class Loggers
    {
        public static CsvLogger<CreateItemLog> CreateItemLogger { get; } = new CsvLogger<CreateItemLog>(@"logs\createitem.csv");
    }
}
