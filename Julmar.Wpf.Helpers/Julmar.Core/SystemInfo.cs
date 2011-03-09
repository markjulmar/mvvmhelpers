using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace JulMar.Core
{
    /// <summary>
    /// Provides operating and hardware system information beyond Environment.OSVersion.
    /// </summary>
    public static class SystemInfo
    {
        private static uint? _doubleClickTime;
        private static readonly OperatingSystemInfo _operatingSystem = new OperatingSystemInfo();

        /// <summary>
        /// Count of physical CPU packages
        /// </summary>
        public static int PhysicalCPUs
        {
            get
            {
                int nRealCpus = 0;

                // Call the right version based on 32 vs. 64-bit.
                if (!Is64BitProcess)
                {
                    var buffer = new SYSTEM_LOGICAL_PROCESSOR_INFOX86[256];
                    int returnLength = buffer.Length * Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFOX86));
                    bool success = NativeMethods.GetLogicalProcessorInformation(buffer, ref returnLength);
                    if (!success)
                    {
                        return 1;
                    }

                    // Run through and try to count the physical processor packages.
                    int nResults = returnLength / Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFOX86));
                    for (int nResult = 0; nResult < nResults; nResult++)
                    {
                        if (buffer[nResult].Relationship == LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorPackage)
                            nRealCpus++;
                    }
                }
                else
                {
                    var buffer = new SYSTEM_LOGICAL_PROCESSOR_INFOX64[256];
                    int returnLength = buffer.Length * Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFOX64));
                    bool success = NativeMethods.GetLogicalProcessorInformation(buffer, ref returnLength);
                    if (!success)
                    {
                        return 1;
                    }

                    int nResults = returnLength / Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFOX86));
                    for (int nResult = 0; nResult < nResults; nResult++)
                    {
                        if (buffer[nResult].Relationship == LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorPackage)
                            nRealCpus++;
                    }
                }

                Debug.Assert(nRealCpus <= Environment.ProcessorCount);
                return nRealCpus;
            }
        }

        /// <summary>
        /// Count of physical cores available. This ignores
        /// HyperThreaded cores reported by Windows.
        /// </summary>
        public static int PhysicalCores
        {
            get
            {
                int nRealCores = 0;

                // Call the right version based on 32 vs. 64-bit.
                if (!Is64BitProcess)
                {
                    var buffer = new SYSTEM_LOGICAL_PROCESSOR_INFOX86[256];
                    int returnLength = buffer.Length*Marshal.SizeOf(typeof (SYSTEM_LOGICAL_PROCESSOR_INFOX86));
                    bool success = NativeMethods.GetLogicalProcessorInformation(buffer, ref returnLength);
                    if (!success)
                    {
                        // This API isn't supported for < Vista. If we hit
                        // one of those, then just return Windows view of the
                        // world.
                        return Environment.ProcessorCount;
                    }

                    // Run through and try to count the physical processor packages.
                    int nResults = returnLength / Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFOX86));
                    for (int nResult = 0; nResult < nResults; nResult++)
                    {
                        if (buffer[nResult].Relationship == LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorCore)
                            nRealCores++;
                    }
                }
                else
                {
                    var buffer = new SYSTEM_LOGICAL_PROCESSOR_INFOX64[256];
                    int returnLength = buffer.Length * Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFOX64));
                    bool success = NativeMethods.GetLogicalProcessorInformation(buffer, ref returnLength);
                    if (!success)
                    {
                        // This API isn't supported for < Vista. If we hit
                        // one of those, then just return Windows view of the
                        // world.
                        return Environment.ProcessorCount;
                    }

                    int nResults = returnLength / Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFOX64));
                    for (int nResult = 0; nResult < nResults; nResult++)
                    {
                        if (buffer[nResult].Relationship == LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorCore)
                            nRealCores++;
                    }
                }

                Debug.Assert(nRealCores <= Environment.ProcessorCount);
                return nRealCores;
            }
        }

        /// <summary>
        /// Provides access to the Processor Affinity mask.
        /// </summary>
        public static int CoresInUse
        {
            get
            {
                int nCores = 0;
                IntPtr cores = Process.GetCurrentProcess().ProcessorAffinity;
                while (cores != IntPtr.Zero)
                {
                    if (((int)cores & 1) == 1)
                    {
                        nCores++;
                    }
                    cores = (IntPtr)((int)cores >> 1);
                }
                return nCores;
            }

            set
            {
                if ((value < 1) || (value > Environment.ProcessorCount))
                {
                    throw new ArgumentException("Illegal number of cores");
                }

                int cores = 1;
                for (int nShift = 0; nShift < value - 1; nShift++)
                {
                    cores = 1 | (cores << 1);
                }

                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)cores;
            }
        }

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
                        _doubleClickTime = NativeMethods.GetDoubleClickTime();
                    }        
                    catch (SecurityException)
                    {
                        _doubleClickTime = 500; // 1/2 second
                    }
                }
                return _doubleClickTime.Value;
            }
        }

        /// <summary>
        /// Returns TRUE if we are running as an x64 process.
        /// </summary>
        public static bool Is64BitProcess
        {
            get
            {
                return Marshal.SizeOf(typeof(IntPtr)) == 8;
            }
        }

        /// <summary>
        /// Returns whether this is a 64-bit version of Windows.
        /// </summary>
        public static bool Is64BitWindows
        {
            get
            {
                try
                {
                    // This appears to be the most reliable way to determine this.
                    string arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine);
                    return !string.IsNullOrEmpty(arch) && arch.Contains("64");
                }
                catch (NotSupportedException)
                {
                    return false;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns true if this is a 32-bit process running on a 64-bit box.
        /// </summary>
        public static bool IsWow64Process
        {
            get
            {
                return Is64BitWindows && !Is64BitProcess;
            }
        }
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
        /// Windows 2000
        /// </summary>
        Windows2000,
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
            get { return NativeMethods.GetSystemMetrics(SM_TABLETPC); }
        }

        /// <summary>
        /// Returns whether the system is "Media Center"
        /// </summary>
        public bool IsMediaCenter
        {
            get { return NativeMethods.GetSystemMetrics(SM_MEDIACENTER); }
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
                    OSVERSIONINFOEX osvi = new OSVERSIONINFOEX();
                    osvi.dwOSVersionInfoSize = Marshal.SizeOf(osvi);
                    if (NativeMethods.GetVersionEx(ref osvi))
                        isEmbedded = (osvi.wSuiteMask & 0x40)==0x40;  // VER_SUITE_EMBEDDEDNT
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

                        return ((PRODUCT_TYPE)productInfo).ToString();
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
            bool isServer = false;
            OSVERSIONINFOEX osvi = new OSVERSIONINFOEX();

            // If browser hosted, then do the best we can.  Assume workstation.
            osvi.dwOSVersionInfoSize = Marshal.SizeOf(osvi);
            if (NativeMethods.GetVersionEx(ref osvi))
                isServer = osvi.wProductType != 0x1;

            // Begin OS detection
            OperatingSystem ver = OperatingSystem.Unknown;
            if (majorVersion == 5)
            {
                switch (minorVersion)
                {
                    // 5.0 = Windows 2000
                    case 0:
                        ver = OperatingSystem.Windows2000;
                        break;
                    // 5.1 = Windows XP
                    case 1:
                        ver = OperatingSystem.WindowsXP;
                        break;
                    // 5.2 = Windows Server 2003, Windows Server 2003R2 or Windows XP 64-bit
                    case 2:
                        ver = OperatingSystem.Windows2003;
                        // Special case 64-bit Windows XP
                        if (!isServer)
                            ver = OperatingSystem.WindowsXP;
                        else
                        {
                            if (NativeMethods.GetSystemMetrics(SM_SERVERR2))
                                ver = OperatingSystem.Windows2003R2;
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
                        if (isServer)
                            ver = OperatingSystem.Windows2008;
                        break;
                    // 6.1 = Windows 7, Windows Server 2008 R2
                    case 1:
                        ver = OperatingSystem.Windows7;
                        if (isServer)
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
        public delegate bool GetProductInfoDelegate(int majorVersion, int minorVersion, int spMajorVersion, int spMinorVersion, out uint productType);

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
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool GetLogicalProcessorInformation([Out] SYSTEM_LOGICAL_PROCESSOR_INFOX86[] buffer, ref int returnLength);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool GetLogicalProcessorInformation([Out] SYSTEM_LOGICAL_PROCESSOR_INFOX64[] buffer, ref int returnLength);
        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();
    }

    // From Winnt.h
    enum PRODUCT_TYPE : uint
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

    enum PROCESSOR_CACHE_TYPE
    {
        /// <summary>
        /// Unified Cache
        /// </summary>
        UnifiedCache     = 0,
        /// <summary>
        /// Processor instruction cache
        /// </summary>
        InstructionCache = 1,
        /// <summary>
        /// Data cache
        /// </summary>
        DataCache        = 2,
        /// <summary>
        /// Trace cache
        /// </summary>
        TraceCache       = 3
    };

    [StructLayout(LayoutKind.Sequential)]
    struct CACHE_DESCRIPTOR
    {
        public byte Level;
        public byte Associativity;
        public short LineSize;
        public int Size;
        [MarshalAs(UnmanagedType.U4)]
        public PROCESSOR_CACHE_TYPE Type;
    };

    enum LOGICAL_PROCESSOR_RELATIONSHIP
    {
        /// <summary>
        /// Logical processors share single processor core.
        /// </summary>
        RelationProcessorCore    = 0,
        /// <summary>
        /// Logical processors part of same NUMA node.
        /// </summary>
        RelationNumaNode         = 1,
        /// <summary>
        /// Processors share cache.
        /// </summary>
        RelationCache            = 2,
        /// <summary>
        /// Processors share physical socket package
        /// </summary>
        RelationProcessorPackage = 3,
    };

    [StructLayout(LayoutKind.Explicit)]
    struct SYSTEM_LOGICAL_PROCESSOR_INFOX86
    {
        [FieldOffset(0)]
        public uint ProcessorMask;
        
        [FieldOffset(4), MarshalAs(UnmanagedType.U4)]
        public LOGICAL_PROCESSOR_RELATIONSHIP Relationship;

        [FieldOffset(8)]
        public byte Flags;

        [FieldOffset(8)]
        public int NodeNumber;

        [FieldOffset(8)]
        public CACHE_DESCRIPTOR CacheDescriptor;

        [FieldOffset(8)]
        public long Reserved1;
        
        [FieldOffset(12)]
        public long Reserved2;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct SYSTEM_LOGICAL_PROCESSOR_INFOX64
    {
        [FieldOffset(0)]
        public uint ProcessorMask;

        [FieldOffset(8), MarshalAs(UnmanagedType.U4)]
        public LOGICAL_PROCESSOR_RELATIONSHIP Relationship;

        [FieldOffset(12)]
        public byte Flags;

        [FieldOffset(12)]
        public int NodeNumber;

        [FieldOffset(12)]
        public CACHE_DESCRIPTOR CacheDescriptor;

        [FieldOffset(12)]
        public long Reserved1;

        [FieldOffset(20)]
        public long Reserved2;
    }

}
