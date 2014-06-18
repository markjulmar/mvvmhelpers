using System;
using System.Linq;
using System.Reflection;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace JulMar.Internal
{
    /// <summary>
    ///     Very simple type conversion
    /// </summary>
    internal static class TypeConverter
    {
        /// <summary>
        ///     Convert source to given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static object Convert(Type type, object source)
        {
            if (source == null)
            {
                return null;
            }

            if (source.GetType() == type)
            {
                return source;
            }

            try
            {
                if (source is string)
                {
                    string value = source.ToString();
                    if (type == typeof(byte))
                    {
                        return byte.Parse(value);
                    }
                    if (type == typeof(sbyte))
                    {
                        return sbyte.Parse(value);
                    }
                    if (type == typeof(short))
                    {
                        return short.Parse(value);
                    }
                    if (type == typeof(ushort))
                    {
                        return ushort.Parse(value);
                    }
                    if (type == typeof(int))
                    {
                        return int.Parse(value);
                    }
                    if (type == typeof(uint))
                    {
                        return uint.Parse(value);
                    }
                    if (type == typeof(long))
                    {
                        return long.Parse(value);
                    }
                    if (type == typeof(ulong))
                    {
                        return ulong.Parse(value);
                    }
                    if (type == typeof(double))
                    {
                        return double.Parse(value);
                    }
                    if (type == typeof(decimal))
                    {
                        return decimal.Parse(value);
                    }
                    if (type == typeof(DateTime))
                    {
                        return DateTime.Parse(value);
                    }
                    if (type == typeof(float))
                    {
                        return float.Parse(value);
                    }
                    if (type == typeof(bool))
                    {
                        return bool.Parse(value);
                    }
                    if (type == typeof(Color))
                    {
                        return ParseColor(value);
                    }
                    if (type == typeof(Brush))
                    {
                        return ParseBrush(value);
                    }
                }
                else
                {
                    if (type == typeof(byte))
                    {
                        return System.Convert.ToByte(source);
                    }
                    if (type == typeof(sbyte))
                    {
                        return System.Convert.ToSByte(source);
                    }
                    if (type == typeof(short))
                    {
                        return System.Convert.ToInt16(source);
                    }
                    if (type == typeof(ushort))
                    {
                        return System.Convert.ToUInt16(source);
                    }
                    if (type == typeof(int))
                    {
                        return System.Convert.ToInt32(source);
                    }
                    if (type == typeof(uint))
                    {
                        return System.Convert.ToUInt32(source);
                    }
                    if (type == typeof(long))
                    {
                        return System.Convert.ToInt64(source);
                    }
                    if (type == typeof(ulong))
                    {
                        return System.Convert.ToUInt64(source);
                    }
                    if (type == typeof(string))
                    {
                        return source.ToString();
                    }
                    if (type == typeof(double))
                    {
                        return System.Convert.ToDouble(source);
                    }
                    if (type == typeof(decimal))
                    {
                        return System.Convert.ToDecimal(source);
                    }
                    if (type == typeof(DateTime))
                    {
                        return System.Convert.ToDateTime(source);
                    }
                    if (type == typeof(float))
                    {
                        return System.Convert.ToSingle(source);
                    }
                    if (type == typeof(bool))
                    {
                        return System.Convert.ToBoolean(source);
                    }
                    if (type == typeof(Color))
                    {
                        return ParseColor(source.ToString());
                    }
                    if (type == typeof(Brush))
                    {
                        return source is Brush ? source : ParseBrush(source.ToString());
                    }
                }

                // Generic conversion, last resort.
                return System.Convert.ChangeType(source, type);
            }
            catch
            {
            }

            return null;
        }

        private static Color ParseColor(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Color cannot be parsed from empty string");
            }
            if (source.StartsWith("#"))
            {
                byte A = 0xff, R = 0xff, G = 0xff, B = 0xff;

                if (source.Length == 7)
                {
                    A = 0xff;
                    R = byte.Parse(source.Substring(1, 2));
                    G = byte.Parse(source.Substring(3, 2));
                    B = byte.Parse(source.Substring(5, 2));
                }
                else if (source.Length == 9)
                {
                    A = byte.Parse(source.Substring(1, 2));
                    R = byte.Parse(source.Substring(3, 2));
                    G = byte.Parse(source.Substring(5, 2));
                    B = byte.Parse(source.Substring(7, 2));
                }

                return Color.FromArgb(A, R, G, B);
            }
            PropertyInfo theColor = typeof(Colors).GetTypeInfo().DeclaredProperties.First(pi => pi.Name == source);
            return (Color)theColor.GetValue(null, null);
        }

        private static Brush ParseBrush(string source)
        {
            return new SolidColorBrush(ParseColor(source));
        }
    }
}