using System.Globalization;
using System.Windows;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    /// <summary>
    /// Associate with a standalone window that has single instance at one time.
    /// </summary>
    public interface ISubWindow
    {
        /// <summary>
        /// Create a new window. Must create a new one since the old one will be unavailable after closed.
        /// </summary>
        /// <returns></returns>
        Window CreateWindow();
        /// <summary>
        /// Get localized title.
        /// </summary>
        /// <param name="culture">Expected language.</param>
        /// <returns>Localized title.</returns>
        string GetTitle(CultureInfo culture);
        /// <summary>
        /// Get the category of the window.
        /// </summary>
        SubWindowCategory Category { get; }
    }
    /// <summary>
    /// Represent the category of a standalone window.
    /// </summary>
    public enum SubWindowCategory
    {
        /// <summary>
        /// Provide overview information of current game.
        /// </summary>
        Overview,
        /// <summary>
        /// Provide statistic information.
        /// </summary>
        Statistics,
        /// <summary>
        /// Provide universal information of the game.
        /// </summary>
        Information
    }
}
