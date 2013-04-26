using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace JulMar.Windows.Serialization
{
    /// <summary>
    /// Class used to serialize/deserialize from XML.  
    /// </summary>
    public static class Xml
    {
        /// <summary>
        /// This method serializes an object or graph into an XML string
        /// </summary>
        /// <param name="instance">Instance to serialize</param>
        /// <param name="knownTypes">Additional known types</param>
        /// <returns>String</returns>
        public static string Serialize(object instance, IEnumerable<Type> knownTypes = null)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (knownTypes == null)
                knownTypes = Enumerable.Empty<Type>();

            string result;
            using (var stream = new MemoryStream())
            {
                var ser = new DataContractSerializer(instance.GetType(), knownTypes);
                ser.WriteObject(stream, instance);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

        /// <summary>
        /// This takes a JSON string and turns it into an object or graph.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">JSON string</param>
        /// <param name="knownTypes">Known types</param>
        /// <returns>Object graph</returns>
        public static T Deserialize<T>(string stream, IEnumerable<Type> knownTypes = null)
        {
            return (T)Deserialize(typeof(T), stream, knownTypes);
        }

        /// <summary>
        /// This takes a JSON string and turns it into an object or graph.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="stream">JSON string</param>
        /// <param name="knownTypes">Known types</param>
        /// <returns>Object graph</returns>
        public static object Deserialize(Type type, string stream, IEnumerable<Type> knownTypes = null)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(stream))
                throw new ArgumentNullException("stream");
            if (knownTypes == null)
                knownTypes = Enumerable.Empty<Type>();

            var bytes = Encoding.Unicode.GetBytes(stream);
            using (var mstream = new MemoryStream(bytes))
            {
                var serializer = new DataContractSerializer(type, knownTypes);
                return serializer.ReadObject(mstream);
            }
        }
    }
}
