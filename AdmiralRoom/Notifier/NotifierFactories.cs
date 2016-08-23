using System;
using System.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom.Notifier
{
    public abstract class NotifierFactories
    {
        private static readonly Dictionary<NotifierType, Type> TypeList = new Dictionary<NotifierType, Type>
        {
            [NotifierType.Ballon] = typeof(BallonNotifierFactory),
            [NotifierType.Toast] = typeof(ToastNotifierFactory),
            [NotifierType.MessageBox] = typeof(MessageBoxNotifierFactory)
        };

        public static NotifierFactory Current { get; private set; }

        private static NotifierType _currentType;
        public static NotifierType CurrentType
        {
            get { return _currentType; }
            set
            {
                if (value == _currentType) return;
                _currentType = value;
                (Current as IDisposable)?.Dispose();
                Current = (NotifierFactory)Activator.CreateInstance(TypeList[_currentType]);
            }
        }

        public static NotifierType DefaultType => ToastNotifierFactory.IsSupported ? NotifierType.Toast : NotifierType.Ballon;
    }

    public enum NotifierType
    {
        Ballon,
        Toast,
        MessageBox
    }
}
