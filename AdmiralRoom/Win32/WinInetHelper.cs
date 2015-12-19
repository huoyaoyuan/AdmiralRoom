using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Huoyaoyuan.AdmiralRoom
{
    static class WinInetHelper
    {
        /// <summary>
        /// 清理Internet缓存文件
        /// </summary>
        /// <seealso cref="https://support.microsoft.com/kb/326201"/>
        [StructLayout(LayoutKind.Sequential)]
        public struct INTERNET_CACHE_ENTRY_INFOA
        {
            public uint dwStructSize;
            public IntPtr lpszSourceUrlName;
            public IntPtr lpszLocalFileName;
            public uint CacheEntryType;
            public uint dwUseCount;
            public uint dwHitRate;
            public uint dwSizeLow;
            public uint dwSizeHigh;
            public FILETIME LastModifiedTime;
            public FILETIME ExpireTime;
            public FILETIME LastAccessTime;
            public FILETIME LastSyncTime;
            public IntPtr lpHeaderInfo;
            public uint dwHeaderInfoSize;
            public IntPtr lpszFileExtension;
            public uint dwReserved;
            public uint dwExemptDelta;
        }
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindFirstUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstUrlCacheGroup(
            int dwFlags,
            int dwFilter,
            IntPtr lpSearchCondition,
            int dwSearchCondition,
            ref long lpGroupId,
            IntPtr lpReserved);
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindNextUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextUrlCacheGroup(
            IntPtr hFind,
            ref long lpGroupId,
            IntPtr lpReserved);
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "DeleteUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteUrlCacheGroup(
            long GroupId,
            int dwFlags,
            IntPtr lpReserved);
        [DllImport(@"wininet",
                    SetLastError = true,
                    CharSet = CharSet.Auto,
                    EntryPoint = "FindFirstUrlCacheEntryA",
                    CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstUrlCacheEntry(
                    [MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern,
                    IntPtr lpFirstCacheEntryInfo,
                    ref int lpdwFirstCacheEntryInfoBufferSize);
        [DllImport(@"wininet",
                    SetLastError = true,
                    CharSet = CharSet.Auto,
                    EntryPoint = "FindNextUrlCacheEntryA",
                    CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextUrlCacheEntry(
                    IntPtr hFind,
                    IntPtr lpNextCacheEntryInfo,
                    ref int lpdwNextCacheEntryInfoBufferSize);
        [DllImport(@"wininet",
                    SetLastError = true,
                    CharSet = CharSet.Auto,
                    EntryPoint = "DeleteUrlCacheEntryA",
                    CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteUrlCacheEntry(
                    IntPtr lpszUrlName);
        public static bool DeleteInternetCache()
        {
            const int CACHEGROUP_SEARCH_ALL = 0x0;
            const int ERROR_NO_MORE_ITEMS = 259;
            const uint CacheEntryType_Cookie = 1048577;
            const uint CacheEntryType_History = 2097153;

            long groupId = 0;
            var cacheEntryInfoBufferSizeInitial = 0;

            var enumHandle = FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);
            if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error()) return false;

            enumHandle = FindFirstUrlCacheEntry(null, IntPtr.Zero, ref cacheEntryInfoBufferSizeInitial);
            if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error()) return false;

            var cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
            var cacheEntryInfoBuffer = Marshal.AllocHGlobal(cacheEntryInfoBufferSize);
            enumHandle = FindFirstUrlCacheEntry(null, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);

            while (true)
            {
                var internetCacheEntry = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(
                    cacheEntryInfoBuffer, typeof(INTERNET_CACHE_ENTRY_INFOA));
                cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;

                var type = internetCacheEntry.CacheEntryType;
                var result = false;

                if (type != CacheEntryType_Cookie && type != CacheEntryType_History)
                {
                    result = DeleteUrlCacheEntry(internetCacheEntry.lpszSourceUrlName);
                }

                if (!result)
                {
                    result = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                }
                if (!result && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                {
                    break;
                }
                if (!result && cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize)
                {
                    cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                    cacheEntryInfoBuffer = Marshal.ReAllocHGlobal(cacheEntryInfoBuffer, (IntPtr)cacheEntryInfoBufferSize);
                    FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                }
            }

            Marshal.FreeHGlobal(cacheEntryInfoBuffer);

            return true;
        }
        public static Task<bool> DeleteInternetCacheAsync() => Task.Factory.StartNew(DeleteInternetCache);
    }
}
