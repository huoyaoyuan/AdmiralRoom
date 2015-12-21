namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Exp
    {
        public int Current { get; set; }
        public int Next { get; set; }
        public int NextLevel { get; set; }
        public int PrevLevel { get; set; }
        public Exp() { }
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
        public override string ToString() => $"{Current}/{NextLevel}";
    }
}
