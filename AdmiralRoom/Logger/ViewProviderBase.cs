using System.Windows.Controls;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    internal class ViewProviderBase : NotificationObject
    {
        public GridViewColumn[] ViewColumns { get; protected set; }
    }
}
