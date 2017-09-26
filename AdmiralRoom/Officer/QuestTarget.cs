using System;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class QuestTarget : NotificationObject, IDisposable
    {
        public ICounter Counter { get; }
        public int QuestId { get; }
        public string Description { get; }
        public QuestPeriod Period { get; }

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

        private QuestTarget _sharedwith;
        private QuestTarget SharedWith
        {
            get
            {
                if (_sharedwith == null)
                    _sharedwith = QuestManager.KnownQuests[sharedwithid]?.MainTarget;
                return _sharedwith;
            }
        }
        private readonly int sharedwithid;
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
        public QuestTarget(ICounter counter, int questid, QuestPeriod period, int max, int sharedwith = 0, string description = "")
        {
            Counter = counter;
            counter.Increased += Increase;
            QuestId = questid;
            Period = period;
            Progress = new LimitedValue(0, max);
            Description = description;
            sharedwithid = sharedwith;
        }

        private void Increase(int n)
        {
            if (IsTook)
            {
                _progress.Current += n;
                _progress.CheckCurrent();
                OnPropertyChanged(nameof(Progress));
                SharedWith?.SharedIncrease(n);
            }
        }

        private void SharedIncrease(int n)
        {
            _progress.Current += n;
            _progress.CheckCurrent();
            OnPropertyChanged(nameof(Progress));
        }

        public void Dispose() => Counter.Increased -= Increase;

        public void SetProgress(int n, bool force)
        {
            if (force || _progress.Current < n)
            {
                _progress.Current = n;
                OnAllPropertyChanged();
            }
        }

        public void Set50() => SetProgress((int)Math.Ceiling(_progress.Max * 0.5), false);
        public void Set80() => SetProgress((int)Math.Ceiling(_progress.Max * 0.8), false);
        public void Set100() => SetProgress(_progress.Max, true);
    }
}
