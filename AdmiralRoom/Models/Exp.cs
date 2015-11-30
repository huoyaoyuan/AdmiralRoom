namespace Huoyaoyuan.AdmiralRoom.Models
{
    public class Exp : NotificationObject
    {
        #region Current
        private int _current;
        public int Current
        {
            get { return _current; }
            set
            {
                if (_current != value)
                {
                    _current = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Next
        private int _next;
        public int Next
        {
            get { return _next; }
            set
            {
                if (_next != value)
                {
                    _next = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NextLevel
        private int _nextlevel;
        public int NextLevel
        {
            get { return _nextlevel; }
            set
            {
                if (_nextlevel != value)
                {
                    _nextlevel = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Percent
        private int _percent;
        public int Percent
        {
            get { return _percent; }
            set
            {
                if (_percent != value)
                {
                    _percent = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

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
