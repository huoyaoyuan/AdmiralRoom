using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    public interface IModule
    {
        string Name { get; }
        void OnCultureChanged(CultureInfo culture);
        IEnumerable<IChildView> ChildViews { get; }
        IEnumerable<IChildWindow> ChildWindows { get; }
        FrameworkElement SettingView { get; }
    }
    public interface IChildView
    {
        FrameworkElement View { get; }
        string ContentId { get; }
        string Title { get; }
        ImageSource Icon { get; }
    }
    public interface IChildWindow
    {
        Type WindowType { get; }
        string Title { get; }
        ImageSource Icon { get; }
    }
}
