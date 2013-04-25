using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace JulMar.Windows.Serialization
{
    /// <summary>
    /// Class used to serialize/deserialize from JSON.  
    /// This was contributed by Paulo Quicoli (pauloquicoli@gmail.com)
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// This method serializes an object or graph into a JSON string
        /// </summary>
        /// <param name="instance">Instance to serialize</param>
        /// <returns>String</returns>
        public static string Serialize(object instance)
        {
            string result;
            using (var stream = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(instance.GetType());
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
        /// <returns>Object graph</returns>
        public static T Deserialize<T>(string stream)
        {
            return (T) Deserialize(typeof(T), stream);
        }

        /// <summary>
        /// This takes a JSON string and turns it into an object or graph.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="stream">JSON string</param>
        /// <returns>Object graph</returns>
        public static object Deserialize(Type type, string stream)
        {
            var bytes = Encoding.Unicode.GetBytes(stream);
            using (var mstream = new MemoryStream(bytes))
            {
                var serializer = new DataContractJsonSerializer(type);
                return serializer.ReadObject(mstream);
            }
        }
    }
}
