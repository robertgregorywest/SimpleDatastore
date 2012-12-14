using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;

namespace SimpleDatastore
{
    internal static class TypeExtensions
    {
        internal static bool IsAPersistentObject(this Type type)
        {
            if (typeof(PersistentObject).IsAssignableFrom(type))
            {
                return true;
            }
            return false;
        }

        internal static bool IsAPersistentObjectList(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(IList<>)) && typeof(PersistentObject).IsAssignableFrom(type.GetGenericArguments()[0]))
            {
                return true;
            }
            return false;
        }

        internal static IEnumerable<PropertyInfo> GetValidProperties(this Type type)
        {
            return type.GetProperties().Where(p => IsValidAttribute(p)).OrderBy(p => p.Name);
        }

        private static bool IsValidAttribute(PropertyInfo property)
        {
            if (Attribute.IsDefined(property, typeof(DataMemberAttribute)))
            {
                return true;
            }
            return false;
        }
    }
}
