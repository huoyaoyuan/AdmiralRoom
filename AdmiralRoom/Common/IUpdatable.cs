namespace Huoyaoyuan.AdmiralRoom
{
    public interface IUpdatable
    {
        void Update();
    }
    public interface IUpdatable<in T>
    {
        void Update(T source);
    }
}
