using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SimpleDatastore.Interfaces;
using static System.Attribute;

namespace SimpleDatastore.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsPersistentObject(this Type type) 
            => typeof(PersistentObject).IsAssignableFrom(type);

        internal static bool IsPersistentObjectEnumerable(this Type type) 
            => type.IsGenericType && typeof(IEnumerable<PersistentObject>).IsAssignableFrom(type);

        internal static IEnumerable<PropertyInfo> PersistedProperties(this Type type,
            SimpleDatastoreOptions.StorageModeOptions storageMode = SimpleDatastoreOptions.StorageModeOptions.Xml)
        {
            return (storageMode == SimpleDatastoreOptions.StorageModeOptions.Xml)
                ? type.GetProperties().Where(p => IsDefined(p, typeof(DataMemberAttribute)))
                : type.GetProperties().Where(p => IsDefined(p, typeof(JsonPropertyNameAttribute)));
        }

        internal static string GetPropertyName(this PropertyInfo property,
            SimpleDatastoreOptions.StorageModeOptions storageMode = SimpleDatastoreOptions.StorageModeOptions.Xml)
        {
            return storageMode == SimpleDatastoreOptions.StorageModeOptions.Xml
                ? ((DataMemberAttribute) GetCustomAttribute(property, typeof(DataMemberAttribute)))?.Name ?? property.Name
                : ((JsonPropertyNameAttribute) GetCustomAttribute(property, typeof(JsonPropertyNameAttribute)))?.Name ?? property.Name;
        }

        internal static bool IsGuid(this Type type) => type == typeof(Guid);

        internal static bool IsString(this Type type) => type == typeof(string);
        
        internal static dynamic CreateRepository(this Type type, Func<Type, dynamic> repoProvider)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(type);
            return repoProvider(repositoryType);
        }
        
        internal static dynamic CreateEnumerableRepository(this Type type, Func<Type, dynamic> repoProvider)
        {
            var elementType = type.GetGenericArguments()[0];
            var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
            return repoProvider(repositoryType);
        }
    }
}
