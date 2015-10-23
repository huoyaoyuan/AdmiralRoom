using System;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class CounterBase : ICounter
    {
        public event Action<int> Increased;

        public void Increase()
        {
            Increased?.Invoke(1);
        }

        public void Increase(int count)
        {
            Increased?.Invoke(count);
        }
    }
}
