using System;
using System.Diagnostics;
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
    }
}
