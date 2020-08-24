using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace SimpleDatastore.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsPersistentObject(this Type type) 
            => typeof(PersistentObject).IsAssignableFrom(type);

        internal static bool IsPersistentObjectEnumerable(this Type type) 
            => type.IsGenericType && typeof(IEnumerable<PersistentObject>).IsAssignableFrom(type);

        internal static IEnumerable<PropertyInfo> PersistedProperties(this Type type)
            => type.GetProperties().Where(p => Attribute.IsDefined(p, typeof(DataMemberAttribute)));

        internal static string GetPropertyName(this PropertyInfo property)
            => ((DataMemberAttribute)Attribute.GetCustomAttribute(property, typeof(DataMemberAttribute)))?.Name 
           ?? property.Name;

        internal static bool IsGuid(this Type type) => type == typeof(Guid);

        internal static bool IsString(this Type type) => type == typeof(string);
    }
}
