using System;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows;

namespace MVVMFolderExplorer.Converters
{
    [StructLayout(LayoutKind.Sequential)]
    struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // Large icon
        public const uint SHGFI_SMALLICON = 0x1; // Small icon

        [DllImport("shell32.dll")]
        static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("User32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr handle);

        public static ImageSource LoadIcon(string name)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            SHGetFileInfo(name, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
            ImageSource imageSource;
            try
            {
                imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(shinfo.hIcon, Int32Rect.Empty, null);
            }
            finally
            {
                DestroyIcon(shinfo.hIcon);
            }
            return imageSource;
        }
    }
}