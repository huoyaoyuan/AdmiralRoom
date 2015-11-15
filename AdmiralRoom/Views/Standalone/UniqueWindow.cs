using System;
using System.Windows;

namespace Huoyaoyuan.AdmiralRoom.Views.Standalone
{
    abstract class UniqueWindow
    {
        public abstract void ShowOrActivate();
        public static void ShowOrActivate(Type windowtype)
        {
            if (!windowtype.IsSubclassOf(typeof(Window)))
            {
                throw new ArgumentException();
            }
            var caller = Activator.CreateInstance(typeof(UniqueWindow<>).MakeGenericType(windowtype)) as UniqueWindow;
            caller.ShowOrActivate();
        }
    }
    sealed class UniqueWindow<T> : UniqueWindow
        where T : Window, new()
    {
        public static T Current { get; private set; }
        public override void ShowOrActivate()
        {
            if (Current == null) Current = new T();
            Current.Show();
            Current.Activate();
        }
    }
}
