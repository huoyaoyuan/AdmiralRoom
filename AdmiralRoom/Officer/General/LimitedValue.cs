namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public struct LimitedValue
    {
        public int Current { get; set; }
        public int Max { get; set; }
        public bool IsMax => Current >= Max;
        public double Percentage => (double)Current / Max;
        public int Shortage => Max - Current;
        public LimitedValue(int current = 0, int max = 0)
        {
            Current = current;
            Max = max;
        }
        public LimitedValue(int[] value)
        {
            Current = value?[0] ?? 0;
            Max = value?[1] ?? 0;
        }
        public void CheckCurrent()
        {
            if (Current > Max) Current = Max;
            if (Current < 0) Current = 0;
        }
        public override string ToString()
        {
            return $"{Current}/{Max}";
        }
    }
}
