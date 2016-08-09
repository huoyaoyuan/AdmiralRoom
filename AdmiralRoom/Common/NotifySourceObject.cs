using System.Runtime.CompilerServices;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom
{
    abstract class NotifySourceObject<TSource> : NotificationObject
        where TSource : class, IUpdatable
    {
        public TSource Source { get; set; }
        protected override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            base.OnPropertyChanged(name);
            Source?.Update();
        }
    }
}
