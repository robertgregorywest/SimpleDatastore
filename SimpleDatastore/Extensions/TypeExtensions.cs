using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace SimpleDatastore.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsAPersistentObject(this Type type)
        {
            return typeof(PersistentObject).IsAssignableFrom(type);
        }

        internal static bool IsAPersistentObjectEnumerable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) && typeof(PersistentObject).IsAssignableFrom(type.GetGenericArguments()[0]);
        }

        internal static IEnumerable<PropertyInfo> GetValidProperties(this Type type)
        {
            return type.GetProperties().Where(p => Attribute.IsDefined(p, typeof(DataMemberAttribute)));
        }
        
        internal static string GetPropertyName(this PropertyInfo property)
        {
            var attribute = (DataMemberAttribute)Attribute.GetCustomAttribute(property, typeof(DataMemberAttribute));

            return  attribute.Name ?? string.Empty;
        }
    }
}
