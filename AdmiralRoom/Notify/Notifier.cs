using System;

namespace Huoyaoyuan.AdmiralRoom
{
    public abstract class Notifier
    {
        public static Notifier Current { get; private set; }
        public abstract void Initialize();
        public abstract void Show(string title, string detail);
        public static void SetNotifier(bool toast)
        {
            (Current as IDisposable)?.Dispose();
            if (toast && ToastNotifier.IsSupported) Current = new ToastNotifier();
            else Current = new BallonNotifier();
            Current.Initialize();
        }
    }
}
