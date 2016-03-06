using System;
using System.Collections.Generic;
using System.Globalization;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    public interface IModule
    {
        string Name { get; }
        void OnCultureChanged(CultureInfo culture);
        IEnumerable<IChildView> ChildViews { get; }
        IEnumerable<IChildWindow> ChildWindows { get; }
        object SettingView { get; }
    }
    public interface IChildView
    {
        object View { get; }
        string ContentId { get; }
        string Title { get; }
    }
    public interface IChildWindow
    {
        Type WindowType { get; }
        string Title { get; }
    }
}
