using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JulMar.Services
{
    /// <summary>
    /// Wrapper around internal services to make available to external clients.
    /// </summary>
    public static class PlatformServices
    {
        /// <summary>
        /// Retrieve list of assemblies loaded into process.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            return Internal.PlatformHelpers.GetAssemblies();
        }

        /// <summary>
        /// Used to perform Type conversions, implemented as platform
        /// specific to take advantage of TypeConverters.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="convertTo">Type to convert to</param>
        /// <returns>New object or null</returns>
        public static object ConvertType(object source, Type convertTo)
        {
            return Internal.PlatformHelpers.ConvertType(source, convertTo);
        }
 
    }
}
