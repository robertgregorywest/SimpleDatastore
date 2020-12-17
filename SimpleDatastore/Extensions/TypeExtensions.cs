using System;
using System.Collections;
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
        {
            return IsAssignableToGenericType(type, typeof(PersistentObject<>));
        }
        
        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
                return true;

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }

        internal static bool IsPersistentObjectEnumerable(this Type type)
        {
            if (!type.IsConstructedGenericType && !typeof(IEnumerable).IsAssignableFrom(type))
            {
                return false;
            }
            
            var typeParams = type.GetGenericArguments();

            return typeParams.Length == 1 && typeParams[0].IsPersistentObject();
            
            //type.IsGenericType && typeof(IEnumerable<PersistentObject<>>).IsAssignableFrom(type);
        }

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
            var repositoryType = typeof(IReadRepository<,>).MakeGenericType(type, type.GetKeyType());
            return repoProvider(repositoryType);
        }
        
        internal static dynamic CreateEnumerableRepository(this Type type, Func<Type, dynamic> repoProvider)
        {
            var elementType = type.GetGenericArguments()[0];
            var repositoryType = typeof(IReadRepository<,>).MakeGenericType(elementType, elementType.GetKeyType());
            return repoProvider(repositoryType);
        }
        
        internal static Type GetKeyType(this Type type) => type.BaseType?.GetGenericArguments()[0];
    }
}
