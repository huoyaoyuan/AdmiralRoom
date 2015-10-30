namespace Huoyaoyuan.AdmiralRoom
{
    public abstract class Notifier
    {
        public static Notifier Current { get; set; }
        public abstract void Initialize();
        public abstract void Show(string title, string detail);
    }
}
