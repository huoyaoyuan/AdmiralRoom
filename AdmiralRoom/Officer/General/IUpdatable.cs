namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public interface IUpdatable<in T>
    {
        void Update(T s);
    }
}
