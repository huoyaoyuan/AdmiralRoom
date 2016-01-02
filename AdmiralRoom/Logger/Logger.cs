namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public abstract class Logger
    {
        protected Logger() { }
        protected abstract void Log(params object[] data);
    }
}
