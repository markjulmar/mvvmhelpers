using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Security;

namespace JulMar.Windows
{
    /// <summary>
    /// Provides operating and hardware system information beyond Environment.OSVersion.
    /// </summary>
    public static class SystemInfo
    {
        private static uint? _doubleClickTime;
        private static readonly OperatingSystemInfo _operatingSystem = new OperatingSystemInfo();

        /// <summary>
        /// Returns operating system information
        /// </summary>
        public static OperatingSystemInfo OperatingSystem
        {
            get { return _operatingSystem; }
        }

        /// <summary>
        /// Time (in msec) for two consecutive button-down events to
        /// equal a double-click
        /// </summary>
        public static uint DoubleClickTime
        {
            get
            {
                if (_doubleClickTime == null)
                {
                    try
                    {
                        _doubleClickTime = GetDoubleClickTime();
                    }        
                    catch (SecurityException)
                    {
                        _doubleClickTime = 500; // 1/2 second
                    }
                }
                return _doubleClickTime.Value;
            }
        }

        // TODO: add CPU info

        [DllImport("user32.dll")]
        static extern uint GetDoubleClickTime();
        
    }

    /// <summary>
    /// Operating system type detected
    /// </summary>
    public enum OperatingSystem
    {
        /// <summary>
        /// Unknown operating system (pre-XP)
        /// </summary>
        Unknown,
        /// <summary>
        /// Windows XP
        /// </summary>
        WindowsXP,
        /// <summary>
        /// Windows Server 2003
        /// </summary>
        Windows2003,
        /// <summary>
        /// Windows Server 2003 R2
        /// </summary>
        Windows2003R2,
        /// <summary>
        /// Windows Vista
        /// </summary>
        WindowsVista,
        /// <summary>
        /// Windows 2008
        /// </summary>
        Windows2008,
        /// <summary>
        /// Windows 7
        /// </summary>
        Windows7,
        /// <summary>
        /// Windows 2008 R2
        /// </summary>
        Windows2008R2
    }

    /// <summary>
    /// Contains information about the running operating system.  Detects between
    /// Windows7 vs. W2K8R2, etc.
    /// </summary>
    public class OperatingSystemInfo
    {
        private OperatingSystem _os = OperatingSystem.Unknown;
        private bool? isEmbedded;
        private string _prodInfo;

        /// <summary>
        /// Returns the running operating system as an enumeration
        /// </summary>
        public OperatingSystem Product
        {
            get
            {
                if (_os == OperatingSystem.Unknown)
                    _os = DetermineOperatingSystem();
                return _os;
            }
        }

        /// <summary>
        /// Returns the active service pack (if any)
        /// </summary>
        public string ServicePack
        {
            get { return Environment.OSVersion.ServicePack; }    
        }

        /// <summary>
        /// Returns whether the system has tablet support
        /// </summary>
        public bool HasTabletSupport
        {
            get { return !BrowserInteropHelper.IsBrowserHosted && NativeMethods.GetSystemMetrics(SM_TABLETPC); }
        }

        /// <summary>
        /// Returns whether the system is "Media Center"
        /// </summary>
        public bool IsMediaCenter
        {
            get { return !BrowserInteropHelper.IsBrowserHosted && NativeMethods.GetSystemMetrics(SM_MEDIACENTER); }
        }

        /// <summary>
        /// Returns whether this is an embedded version of Windows
        /// </summary>
        public bool IsEmbedded
        {
            get
            {
                if (isEmbedded == null)
                {
                    isEmbedded = false; 
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        OSVERSIONINFOEX osvi = new OSVERSIONINFOEX();
                        osvi.dwOSVersionInfoSize = Marshal.SizeOf(osvi);
                        if (NativeMethods.GetVersionEx(ref osvi))
                            isEmbedded = (osvi.wSuiteMask & 0x40)==0x40;  // VER_SUITE_EMBEDDEDNT
                    }
                }

                return isEmbedded.Value;
            }
        }

        /// <summary>
        /// Disallow creation of object
        /// </summary>
        internal OperatingSystemInfo()
        {
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Product, ServicePack, ProductInfo);
        }

        private const int SM_TABLETPC = 86;
        private const int SM_MEDIACENTER = 87;
        private const int SM_SERVERR2 = 89;

        /// <summary>
        /// Returns the product information (Home, Ultimate, etc.)
        /// </summary>
        public string ProductInfo
        {
            get
            {
                if (_prodInfo == null)
                {
                    if (Environment.OSVersion.Version.Major >= 6)
                        _prodInfo = GetProductInfo();
                    else
                    {
                        if (IsMediaCenter)
                            _prodInfo = "Media Center";
                        else if (HasTabletSupport)
                            _prodInfo = "Tablet";
                        else if (IsEmbedded)
                            _prodInfo = "Embedded";
                    }
                }

                return _prodInfo;
            }
        }

        /// <summary>
        /// Retrieves the product info
        /// </summary>
        /// <returns></returns>
        private static string GetProductInfo()
        {
            IntPtr hDll = NativeMethods.LoadLibrary("kernel32.dll");    
            if (hDll != IntPtr.Zero)
            {
                try
                {
                    IntPtr gpiFunc = NativeMethods.GetProcAddress(hDll, "GetProductInfo");
                    if (gpiFunc != IntPtr.Zero)
                    {
                        uint productInfo;
                        var getProductInfo = (NativeMethods.GetProductInfoDelegate)Marshal.GetDelegateForFunctionPointer(gpiFunc, typeof(NativeMethods.GetProductInfoDelegate));
                        if (!getProductInfo.Invoke(6, 1, 0, 0, out productInfo))
                            return string.Empty;

                        return ((ProductType)productInfo).ToString();
                    }
                }
                finally
                {
                    NativeMethods.FreeLibrary(hDll);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// This retrieves the operating system
        /// </summary>
        /// <returns></returns>
        private static OperatingSystem DetermineOperatingSystem()
        {
            int majorVersion = Environment.OSVersion.Version.Major;
            int minorVersion = Environment.OSVersion.Version.Minor;
            bool isServer = false, isBrowserHosted = BrowserInteropHelper.IsBrowserHosted;
            OSVERSIONINFOEX osvi = new OSVERSIONINFOEX();

            // If browser hosted, then do the best we can.  Assume workstation.
            if (!isBrowserHosted)
            {
                osvi.dwOSVersionInfoSize = Marshal.SizeOf(osvi);
                if (NativeMethods.GetVersionEx(ref osvi))
                    isServer = osvi.wProductType != 0x1;
            }

            // Begin OS detection
            OperatingSystem ver = OperatingSystem.Unknown;
            if (majorVersion == 5)
            {
                switch (minorVersion)
                {
                    // 5.0 = Windows 200, not allowed for WPF
                    case 0:
                        break;
                    // 5.1 = Windows XP
                    case 1:
                        ver = OperatingSystem.WindowsXP;
                        break;
                    // 5.2 = Windows Server 2003, Windows Server 2003R2 or Windows XP 64-bit
                    case 2:
                        ver = OperatingSystem.Windows2003;
                        if (!isBrowserHosted)
                        {
                            // Special case 64-bit Windows XP
                            if (!isServer)
                                ver = OperatingSystem.WindowsXP;
                            else
                            {
                                if (NativeMethods.GetSystemMetrics(SM_SERVERR2))
                                    ver = OperatingSystem.Windows2003R2;
                            }
                        }
                        break;
                }
            }
            else if (majorVersion == 6)
            {
                switch (minorVersion)
                {
                    // 6.0 = Windows Vista, Windows Server 2008
                    case 0:
                        ver = OperatingSystem.WindowsVista;
                        if (!isBrowserHosted && isServer)
                            ver = OperatingSystem.Windows2008;
                        break;
                    // 6.1 = Windows 7, Windows Server 2008 R2
                    case 1:
                        ver = OperatingSystem.Windows7;
                        if (!isBrowserHosted && isServer)
                            ver = OperatingSystem.Windows2008R2;
                        break;
                }
            }
            return ver;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct OSVERSIONINFOEX
    {
        public int dwOSVersionInfoSize;
        public int dwMajorVersion;
        public int dwMinorVersion;
        public int dwBuildNumber;
        public int dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;
        public ushort wServicePackMajor;
        public ushort wServicePackMinor;
        public ushort wSuiteMask;
        public byte wProductType;
        public byte wReserved;
    }

    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetVersionEx(ref OSVERSIONINFOEX osvi);
        [DllImport("user32.dll")]
        public static extern bool GetSystemMetrics(int nIndex);
        public delegate bool GetProductInfoDelegate(int majorVersion, int minorVersion, int spMajorVersion, int spMinorVersion, out uint productType);
    }

    // From Winnt.h
    enum ProductType : uint
    {
        Undefined   = 0,
        Ultimate    = 1,
        HomeBasic   = 2,
        HomePremium = 3,
        Enterprise  = 4,
        HomeBasicN  = 5,
        Business    = 6,
        StandardServer = 7,
        DataCenterServer = 8,
        SmallBusinessServer = 9,
        EnterpriseServer = 10,
        Starter = 11,
        DataCenterServerCore = 12,
        StandardServerCore = 13,
        EnterpriseServerCore = 14,
        EnterpriseServerIA64 = 15,
        BusinessN   = 16,
        WebServer   = 17,
        ClusterServer = 18,
        HomeServer = 19,
        StorageExpressServer = 20,
        StorageStandardServer = 21,
        StorageWorkgroupServer = 22,
        StorageEnterpriseServer = 23,
        ServerForSmallBusiness = 24,
        SmallBusinessServerPremium = 25,
        Unlicensed = 0xabcdabcd
    }
}
