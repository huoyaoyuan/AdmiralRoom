using System.Globalization;
using System.Windows;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    /// <summary>
    /// Associate with a subview that can be docked in the main window.
    /// </summary>
    public interface ISubView
    {
        /// <summary>
        /// The sub view.
        /// </summary>
        UIElement View { get; }
        /// <summary>
        /// ContentID used by DockingManager to save the layout. Must be unique.
        /// </summary>
        string ContentID { get; }
        /// <summary>
        /// Get localized title.
        /// </summary>
        /// <param name="culture">Expected language.</param>
        /// <returns>Localized title.</returns>
        string GetTitle(CultureInfo culture);
    }
}
