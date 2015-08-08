using System;
using System.Runtime.InteropServices;

namespace Huoyaoyuan.AdmiralRoom
{
    internal static class NativeMethods
    {
        [DllImport("Avrt.dll")]
        public static extern IntPtr AvSetMmThreadCharacteristics(string taskName, ref uint taskIndex);
    }
}
