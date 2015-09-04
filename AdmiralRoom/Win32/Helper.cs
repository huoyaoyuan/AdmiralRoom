using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Huoyaoyuan.AdmiralRoom
{
    internal static class Helper
    {
        public static bool IsWin8OrGreater =>
            (Environment.OSVersion.Version.Major == 6 &&
            Environment.OSVersion.Version.Minor >= 2)
            || Environment.OSVersion.Version.Major > 6;
        public static void SetIEEmulation(int mode)
        {
            try
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                    Process.GetCurrentProcess().ProcessName+".exe",
                    mode, RegistryValueKind.DWord);
            }
            catch { }
        }
        public static void SetGPURendering(bool enable)
        {
            try
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_GPU_RENDERING",
                    Process.GetCurrentProcess().ProcessName + ".exe",
                    enable ? 1 : 0, RegistryValueKind.DWord);
            }
            catch { }
        }
        public static void SetMMCSSTask()
        {
            uint index = 0;
            NativeMethods.AvSetMmThreadCharacteristics("Games", ref index);
        }
        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        public static bool RefreshIESettings(string strProxy)//strProxy为代理IP:端口 
        {
            const int INTERNET_OPTION_PROXY = 38;
            const int INTERNET_OPEN_TYPE_PROXY = 3;
            const int INTERNET_OPEN_TYPE_DIRECT = 1;
            Struct_INTERNET_PROXY_INFO struct_IPI;
            // Filling in structure 
            struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
            struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");
            // Allocating memory 
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));
            if (string.IsNullOrEmpty(strProxy) || strProxy.Trim().Length == 0)
            {
                strProxy = string.Empty;
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
            }
            // Converting structure to IntPtr 
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);
            return InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));
        }
    }
}
