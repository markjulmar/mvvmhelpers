using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JulMar.Core.Extensions
{
    /// <summary>
    /// This class clones object values without requiring Serializable objects.
    /// Warning: it uses reflection so it will not necessarily be the fastest mechanism
    /// available.
    /// 
    /// The object being cloned must have a public, default constructor.
    /// </summary>
    public static class ObjectCloner
    {
        /// <summary>
        /// Clones an object by doing a full deep copy of every field and property.
        /// </summary>
        /// <param name="source">Object to clone</param>
        /// <returns>Cloned copy</returns>
        public static T Clone<T>(T source)
        {
            return (T) InternalClone(source, new Dictionary<object, object>());
        }

        /// <summary>
        /// Internal method to clone an object
        /// </summary>
        private static object InternalClone(object entity, Dictionary<object, object> refValues)
        {
            // Null? No work
            if (entity == null)
                return null;

            Type entityType = entity.GetType();

            // Special case value-types; they are passed by value so we have our
            // own copy right here.
            if (entityType.IsValueType)
                return entity;

            // Clone strings (special case)
            if (entityType == typeof(string))
                return ((string)entity).Clone();

            // See if we've seen this object already.  If so, return the clone.
            if (refValues.ContainsKey(entity))
                return refValues[entity];

            // If the type is [Serializable], then try that approach first; if serializable
            // fails (a child object is *not* serializable) then we will continue on.
            if (entityType.GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
            {
                try
                {
                    using(var memoryStream = new MemoryStream(4096))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(memoryStream, entity);
                        memoryStream.Position = 0;
                        object clone = binaryFormatter.Deserialize(memoryStream);
                        refValues[entity] = clone;
                        return clone;
                    }
                }
                catch (Exception)
                {
                }
            }

            // If the element is an array, then copy it.
            if (entityType.IsArray)
            {
                Array copy = (Array) ((Array) entity).Clone();
                if (copy.Rank > 1)
                {
                    for (int rank = 0; rank < copy.Rank; rank++)
                    {
                        for (int i = copy.GetLowerBound(rank); i <= copy.GetUpperBound(rank); i++)
                            copy.SetValue(InternalClone(copy.GetValue(rank, i), refValues), rank, i);
                    }
                }
                else
                {
                    for (int i = copy.GetLowerBound(0); i <= copy.GetUpperBound(0); i++)
                    {
                        object value = copy.GetValue(i);
                        copy.SetValue(InternalClone(value, refValues), i);
                    }
                }
                refValues[entity] = copy;
                return copy;
            }

            // Dictionary type
            if (entity is IDictionary)
            {
                IDictionary dictionary = (IDictionary) entity;
                IDictionary clone = (IDictionary) Activator.CreateInstance(entityType);
                foreach (var key in dictionary.Keys)
                {
                    object keyCopy = InternalClone(key, refValues);
                    object valCopy = InternalClone(dictionary[key], refValues);
                    clone.Add(keyCopy, valCopy);
                }
                refValues[entity] = clone;
                return clone;
            }

            // IList type
            if (entity is IList)
            {
                IList list = (IList) entity;
                IList clone = (IList) Activator.CreateInstance(entityType);
                foreach (var value in list)
                {
                    object valCopy = InternalClone(value, refValues);
                    clone.Add(valCopy);
                }
                refValues[entity] = list;
                return list;
            }

            // No obvious way to copy the object - do a field-by-field copy
            object result = Activator.CreateInstance(entityType);

            // Save off the reference
            refValues[entity] = result;

            // Walk through all the fields - this will capture auto-properties as well.
            foreach (FieldInfo field in entityType.GetFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public)) 
              field.SetValue(result, InternalClone(field.GetValue(entity), refValues));

            return result;
        }
    }
}
