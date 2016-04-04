using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    public interface IModule
    {
        string Name { get; }
        void OnCultureChanged(CultureInfo culture);
        void Unload();
        IEnumerable<IChildView> ChildViews { get; }
        IEnumerable<IChildWindow> ChildWindows { get; }
        FrameworkElement SettingView { get; }
    }
    public interface IChildView
    {
        FrameworkElement View { get; }
        string ContentId { get; }
        string Title { get; }
    }
    public interface IChildWindow
    {
        Type WindowType { get; }
        string Title { get; }
    }
}
