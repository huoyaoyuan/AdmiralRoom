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

        public QuestTarget(ICounter counter, int questid, QuestPeriod period, int max, string description = "")
        {
            Counter = counter;
            counter.Increased += Increase;
            QuestId = questid;
            Period = period;
            Progress = new LimitedValue(0, max);
            Description = description;
        }

        void Increase(int n)
        {
            if (Staff.Current.Quests.QuestInProgress[QuestId] != null)
            {
                _progress.Current += n;
                OnPropertyChanged("Progress");
            }
        }

        public void Dispose() => Counter.Increased -= Increase;

        public void SetProgress(int n)
        {
            if (_progress.Current < n)
            {
                _progress.Current = n;
                OnAllPropertyChanged();
            }
        }

        public void Set50() => SetProgress((int)Math.Ceiling(_progress.Max * 0.5));
        public void Set80() => SetProgress((int)Math.Ceiling(_progress.Max * 0.8));
    }
}
