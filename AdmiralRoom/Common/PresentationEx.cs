using System.Windows;

namespace Huoyaoyuan.AdmiralRoom
{
    public static class PresentationEx
    {
        public static T TryFreeze<T>(this T freezable)
            where T : Freezable
        {
            if (freezable.CanFreeze) freezable.Freeze();
            return freezable;
        }
    }
}
