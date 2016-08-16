namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Exp
    {
        public int Current { get; }
        public int Next { get; }
        public int NextLevel { get; }
        public int PrevLevel { get; }
        public bool IsOverflow { get; }
        public Exp(int[] arr, int level, bool isShipExp)
        {
            Current = arr[0];
            if (isShipExp)//舰娘exp
            {
                Next = arr[1];
                NextLevel = Current + Next;
                PrevLevel = ConstData.GetShipExp(level);
            }
            else//提督exp
            {
                NextLevel = arr[1];
                if (NextLevel != 0)
                    Next = NextLevel - Current;
                PrevLevel = ConstData.GetAdmiralExp(level);
            }
        }
        public Exp(int exp, int level)//提督exp
        {
            Current = exp;
            NextLevel = ConstData.GetAdmiralExp(level + 1);
            PrevLevel = ConstData.GetAdmiralExp(level);
            if (NextLevel != 0)
                Next = NextLevel - Current;
            else IsOverflow = true;
        }
        public override string ToString() => $"{Current}/{NextLevel}";
    }
}
