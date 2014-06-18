using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Windows.ApplicationModel;

namespace JulMar.Internal
{
    /// <summary>
    /// This is the required Windows .NET desktop platform helper for
    /// services we need in order to make the system bootstrap on
    /// multiple platforms.
    /// </summary>
    public static class PlatformHelpers
    {
        /// <summary>
        /// Retrieve the list of assemblies shipped with the application.
        /// </summary>
        /// <returns>Enumerable list of assemblies</returns>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            var folder = Package.Current.InstalledLocation;
            return from file in folder.GetFilesAsync().AsTask().Result 
                   where file.FileType == ".dll" 
                   select new AssemblyName(file.DisplayName) into assemblyName 
                   select Assembly.Load(assemblyName);
        }

        /// <summary>
        /// Converts an object from one type to another.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="convertTo">Type to convert to</param>
        /// <returns>New object of the given type</returns>
        public static object ConvertType(object source, Type convertTo)
        {
            return TypeConverter.Convert(convertTo, source);
        }
    }
}
