using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Huoyaoyuan.AdmiralRoom
{
    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected virtual void OnAllPropertyChanged() => OnPropertyChanged(null);
    }
}
