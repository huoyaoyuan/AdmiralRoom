using System.Linq;
using System.Text;
using Meowtrix;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class QuestInfo : NotificationObject, IIdentifiable<int>
    {
        public int Id => MainTarget.QuestId;
        public QuestTarget[] Targets { get; set; }
        public QuestTarget MainTarget { get; set; }
        public LimitedValue Progress => MainTarget.Progress;
        public double Percentage => Targets.Select(x => x.Progress.Percentage).Average();
        public QuestInfo() { }
        public QuestInfo(QuestTarget target)
        {
            Targets = new[] { target };
            MainTarget = target;
            SetEvents();
        }
        public QuestInfo(QuestTarget[] targets, int main)
        {
            Targets = targets;
            MainTarget = targets[main];
            SetEvents();
        }
        private void SetEvents()
        {
            foreach (var target in Targets)
            {
                target.PropertyChanged += (_, __) => OnAllPropertyChanged();
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var target in Targets)
            {
                sb.Append(target.Description);
                sb.Append(' ');
                sb.AppendLine(target.Progress.ToString());
            }
            return sb.ToString();
        }
        public void Set50()
        {
            if (Targets.Length == 1) MainTarget.Set50();
        }
        public void Set80()
        {
            if (Targets.Length == 1) MainTarget.Set80();
        }
        public void Set100()
        {
            Targets.ForEach(x => x.Set100());
        }
        public void SetIsTook(bool v)
        {
            foreach (var target in Targets)
                target.IsTook = v;
        }
        public static implicit operator QuestInfo(QuestTarget target) => new QuestInfo(target);
    }
}
