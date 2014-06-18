using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            // Add additional ones from the path.
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(path))
            {
                foreach (string dll in Directory.GetFiles(path, "*.dll"))
                {
                    Assembly asm = null;
                    try
                    {
                        asm = Assembly.LoadFrom(dll);
                    }
                    catch (FileLoadException)
                    {
                    }
                    catch (BadImageFormatException)
                    {
                    }

                    if (asm != null)
                        assemblies.Add(asm);
                }
            }

            return assemblies.Distinct();
        }

        /// <summary>
        /// Converts an object from one type to another.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="convertTo">Type to convert to</param>
        /// <returns>New object of the given type</returns>
        public static object ConvertType(object source, Type convertTo)
        {
            if (source == null)
                return null;

            Type sourceType = source.GetType();
            TypeConverter converter = TypeDescriptor.GetConverter(sourceType);
            if (converter.CanConvertTo(convertTo))
                return converter.ConvertTo(source, convertTo);

            converter = TypeDescriptor.GetConverter(convertTo);
            if (converter.CanConvertFrom(sourceType))
                return converter.ConvertFrom(source);

            // No conversion available
            return null;
        }
    }
}
