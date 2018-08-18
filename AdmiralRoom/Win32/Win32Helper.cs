﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;

#pragma warning disable CC0108
#pragma warning disable CC0074
#pragma warning disable CC0052

namespace Huoyaoyuan.AdmiralRoom
{
    internal static class Win32Helper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT minPosition;
            public POINT maxPosition;
            public RECT normalPosition;
        }
        public static void SetIEEmulation(int mode)
        {
            //try
            //{
            //    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
            //        Process.GetCurrentProcess().ProcessName + ".exe",
            //        mode, RegistryValueKind.DWord);
            //}
            //catch { }
        }
        public static void SetGPURendering(bool enable)
        {
            //try
            //{
            //    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_GPU_RENDERING",
            //        Process.GetCurrentProcess().ProcessName + ".exe",
            //        enable ? 1 : 0, RegistryValueKind.DWord);
            //}
            //catch { }
        }
        [DllImport("Avrt.dll")]
        public static extern IntPtr AvSetMmThreadCharacteristics(string taskName, ref uint taskIndex);
        public static void SetMMCSSTask()
        {
            uint index = 0;
            AvSetMmThreadCharacteristics("Games", ref index);
        }
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        public static bool RefreshProxySetting(string address, int port)//strProxy为代理IP:端口
        {
            CefSharp.CefSharpSettings.Proxy = new CefSharp.ProxyOptions(address, port.ToString());
            return true;
        }
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
        public static void SetRestoreWindowPosition(Window window)
        {
            var hwnd = (HwndSource.FromVisual(window) as HwndSource).Handle;
            GetWindowPlacement(hwnd, out var placement);
            var AppFilename = Process.GetCurrentProcess().MainModule.FileName;
            IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINDOWPLACEMENT)));
            try
            {
                Marshal.StructureToPtr(placement, buffer, false);
                byte[] data = new byte[Marshal.SizeOf(typeof(WINDOWPLACEMENT))];
                Marshal.Copy(buffer, data, 0, Marshal.SizeOf(typeof(WINDOWPLACEMENT)));
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ApplicationFrame\Positions\" + AppFilename, "PositionObject", data, RegistryValueKind.Binary);
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ApplicationFrame\Positions\" + AppFilename, "Version", 3, RegistryValueKind.DWord);
            }
            catch { }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        public static void GetRestoreWindowPosition(Window window)
        {
            var hwnd = (HwndSource.FromVisual(window) as HwndSource).Handle;
            WINDOWPLACEMENT placement;
            var AppFilename = Process.GetCurrentProcess().MainModule.FileName;
            try
            {
                byte[] data = (byte[])Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ApplicationFrame\Positions\" + AppFilename, "PositionObject", null);
                IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINDOWPLACEMENT)));
                Marshal.Copy(data, 0, buffer, Marshal.SizeOf(typeof(WINDOWPLACEMENT)));
                placement = Marshal.PtrToStructure<WINDOWPLACEMENT>(buffer);
                Marshal.FreeHGlobal(buffer);
            }
            catch
            {
                return;
            }
            SetWindowPlacement(hwnd, ref placement);
        }
    }
}
