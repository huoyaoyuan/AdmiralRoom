namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Exp
    {
        public int Current { get; set; }
        public int Next { get; set; }
        public int NextLevel { get; set; }
        public int Percent { get; set; }
        public Exp()
        {
            Current = 0;
            Next = 0;
            NextLevel = 0;
            Percent = -1;
        }
        public Exp(int[] arr)
        {
            Current = arr[0];
            if (arr.Length >= 3)//舰娘exp
            {
                Next = arr[1];
                NextLevel = Current + Next;
                Percent = arr[2];
            }
            else//提督exp
            {
                NextLevel = arr[1];
                if (NextLevel != 0)
                    Next = NextLevel - Current;
                Percent = -1;
            }
        }
        public override string ToString()
        {
            return $"{Current}/{NextLevel}";
        }
    }
}
