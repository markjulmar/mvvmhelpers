using System;

namespace JulMar.Windows.Interactivity.Internal
{
    /// <summary>
    /// Very simple type conversion
    /// </summary>
    static class TypeConverter
    {
        /// <summary>
        /// Convert source to given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static object Convert(Type type, object source)
        {
            if (source == null)
                return null;

            if (source.GetType() == type)
                return source;

            try
            {
                if (source is string)
                {
                    string value = source.ToString();
                    if (type == typeof(byte))
                        return byte.Parse(value);
                    else if (type == typeof(sbyte))
                        return sbyte.Parse(value);
                    else if (type == typeof(short))
                        return short.Parse(value);
                    else if (type == typeof(ushort))
                        return ushort.Parse(value);
                    else if (type == typeof(int))
                        return int.Parse(value);
                    else if (type == typeof(uint))
                        return uint.Parse(value);
                    else if (type == typeof(long))
                        return long.Parse(value);
                    else if (type == typeof(ulong))
                        return ulong.Parse(value);
                    else if (type == typeof(double))
                        return double.Parse(value);
                    else if (type == typeof(decimal))
                        return decimal.Parse(value);
                    else if (type == typeof(DateTime))
                        return DateTime.Parse(value);
                    else if (type == typeof(float))
                        return float.Parse(value);
                    else if (type == typeof(bool))
                        return bool.Parse(value);
                }
                else
                {
                    if (type == typeof(byte))
                        return System.Convert.ToByte(source);
                    else if (type == typeof(sbyte))
                        return System.Convert.ToSByte(source);
                    else if (type == typeof(short))
                        return System.Convert.ToInt16(source);
                    else if (type == typeof(ushort))
                        return System.Convert.ToUInt16(source);
                    else if (type == typeof(int))
                        return System.Convert.ToInt32(source);
                    else if (type == typeof(uint))
                        return System.Convert.ToUInt32(source);
                    else if (type == typeof(long))
                        return System.Convert.ToInt64(source);
                    else if (type == typeof(ulong))
                        return System.Convert.ToUInt64(source);
                    else if (type == typeof(string))
                        return source.ToString();
                    else if (type == typeof(double))
                        return System.Convert.ToDouble(source);
                    else if (type == typeof(decimal))
                        return System.Convert.ToDecimal(source);
                    else if (type == typeof(DateTime))
                        return System.Convert.ToDateTime(source);
                    else if (type == typeof(float))
                        return System.Convert.ToSingle(source);
                    else if (type == typeof(bool))
                        return System.Convert.ToBoolean(source);
                }

                // Generic conversion, last resort.
                return System.Convert.ChangeType(source, type);
            }
            catch
            {
            }

            return null;
        }
    }
}
