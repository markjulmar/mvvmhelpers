using System;
using System.Collections.Generic;
using System.Reflection;

namespace JulMar.Internal
{
    /// <summary>
    /// This is the definition for the required platform helper
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
            throw new NotSupportedException();
        } 

        /// <summary>
        /// Converts an object from one type to another.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="convertTo">Type to convert to</param>
        /// <returns>New object of the given type</returns>
        public static object ConvertType(object source, Type convertTo)
        {
            throw new NotImplementedException();
        }
    }
}

