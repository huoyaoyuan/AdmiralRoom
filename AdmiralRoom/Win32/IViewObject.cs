using System;
using System.Runtime.InteropServices;

#pragma warning disable CC0074

namespace Huoyaoyuan.AdmiralRoom.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal class RECT
    {
        public int left;
        public int top;
        public int width;
        public int height;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal class DVTARGETDEVICE
    {
        public ushort tdSize;
        public uint tdDeviceNameOffset;
        public ushort tdDriverNameOffset;
        public ushort tdExtDevmodeOffset;
        public ushort tdPortNameOffset;
        public byte tdData;
    }
    [ComImport, Guid("0000010d-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IViewObject
    {
        [PreserveSig]
        int Draw(
            [In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect,
            int lindex,
            IntPtr pvAspect,
            [In] DVTARGETDEVICE ptd,
            IntPtr hdcTargetDev,
            IntPtr hdcDraw,
            [In] RECT lprcBounds,
            [In] RECT lprcWBounds,
            IntPtr pfnContinue,
            [In] IntPtr dwContinue);
    }
}
