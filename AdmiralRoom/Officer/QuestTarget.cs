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

        protected QuestTarget() { }

        public QuestTarget(ICounter counter)
        {
            Counter = counter;
            counter.Increased += Increase;
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
    }
}
