using System.Linq;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class QuestTargetSet : NotifyBase
    {
        public QuestTarget[] Targets { get; set; }
        public QuestTarget MainTarget { get; set; }
        public LimitedValue Progress => MainTarget.Progress;
        public double Percentage => Targets.ArrayOperation(x => x.Progress.Percentage).Average();
        public QuestTargetSet(QuestTarget target)
        {
            Targets = new[] { target };
            MainTarget = target;
            SetEvents();
        }
        public QuestTargetSet(QuestTarget[] targets, int main)
        {
            Targets = targets;
            MainTarget = targets[main];
            SetEvents();
        }
        void SetEvents()
        {
            foreach (var target in Targets) 
            {
                target.PropertyChanged += (_, __) => OnAllPropertyChanged();
            }
        }
    }
}
