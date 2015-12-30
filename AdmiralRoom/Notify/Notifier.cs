using System;

#pragma warning disable CC0022

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
            Current = toast && ToastNotifier.IsSupported ? (Notifier)new ToastNotifier() : new BallonNotifier();
            Current.Initialize();
        }
    }
}
