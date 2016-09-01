using System.Collections;
using System.Windows.Data;
using System.Windows.Threading;

namespace Huoyaoyuan.AdmiralRoom
{
    public static class DispatcherHelper
    {
        public static Dispatcher UIDispatcher { get; internal set; }
        public static T WithSyncBindingEnabled<T>(this T collection)
            where T : IEnumerable
        {
            BindingOperations.EnableCollectionSynchronization(collection, new object());
            return collection;
        }
    }
}
