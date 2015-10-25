using System;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class QuestTarget : NotifyBase, IDisposable
    {
        public ICounter Counter { get; private set; }
        public int QuestId { get; set; }
        public string Description { get; set; }
        public QuestPeriod Period { get; set; }

        #region Progress
        private LimitedValue _progress;
        public LimitedValue Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private QuestTarget SharedWith;
        private bool _istook;
        public bool IsTook
        {
            get
            {
                if (Staff.Current.Quests.QuestInProgress != null)
                    _istook = Staff.Current.Quests.QuestInProgress?[QuestId] != null;
                return _istook;
            }
            set
            {
                _istook = value;
            }
        }
        public QuestTarget(ICounter counter, int questid, QuestPeriod period, int max, QuestTarget sharedwith = null, string description = "")
        {
            Counter = counter;
            counter.Increased += Increase;
            QuestId = questid;
            Period = period;
            Progress = new LimitedValue(0, max);
            Description = description;
            SharedWith = sharedwith;
        }

        void Increase(int n)
        {
            if (IsTook)
            {
                _progress.Current += n;
                OnPropertyChanged("Progress");
                SharedWith?.SharedIncrease(n);
            }
        }

        void SharedIncrease(int n)
        {
            _progress.Current += n;
            OnPropertyChanged("Progress");
        }

        public void Dispose() => Counter.Increased -= Increase;

        public void SetProgress(int n)
        {
            _progress.Current = n;
            OnAllPropertyChanged();
        }

        public void Set50() => SetProgress((int)Math.Ceiling(_progress.Max * 0.5));
        public void Set80() => SetProgress((int)Math.Ceiling(_progress.Max * 0.8));
    }
}
