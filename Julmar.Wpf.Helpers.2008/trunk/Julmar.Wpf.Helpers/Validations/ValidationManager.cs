using System;
using System.Collections.Generic;
using System.Reflection;

namespace JulMar.Windows.Validations
{
    /// <summary>
    /// This class performs validations on properties decorated with IValidator attribute types.
    /// </summary>
    public static class ValidationManager
    {
        /// <summary>
        /// This validates the given property, or all properties if name is null/empty.
        /// </summary>
        /// <param name="name">Property name, can be null/empty</param>
        /// <param name="instance">Object instance</param>
        /// <returns>List of errors separated by newlines</returns>
        public static string Validate(string name, object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            Type type = instance.GetType();
            var errorList = new List<string>();

            if (!string.IsNullOrEmpty(name))
            {
                ValidateProperty(name, instance, errorList);
            }
            else
            {
                foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    ValidateProperty(pi.Name, instance, errorList);
            }

            return string.Join(Environment.NewLine, errorList.ToArray());
        }

        static void ValidateProperty(string name, object instance, ICollection<string> errorList)
        {
            Type type = instance.GetType();
            PropertyInfo pi = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi != null)
            {
                object value = pi.GetValue(instance, null);
                foreach (Attribute att in pi.GetCustomAttributes(true))
                {
                    var iv = att as IValidator;
                    if (iv != null)
                    {
                        string err = iv.Validate(name, value);
                        if (!string.IsNullOrEmpty(err))
                            errorList.Add(err);
                    }
                }
            }
        }
    }
}
