using System;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public interface ICounter
    {
        void Increase();
        void Increase(int count);
        event Action<int> Increased;
    }
}
