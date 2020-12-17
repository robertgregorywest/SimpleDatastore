using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using static SimpleDatastore.SimpleDatastoreOptions;

namespace SimpleDatastore
{
    // ReSharper disable once UnusedTypeParameter
    // Not closing the type so that DI container can resolve correctly
    public class ItemResolverJson<T, TKey, TElement> : IItemResolver<T, TKey, JsonElement> 
        where T : PersistentObject<TKey> where TKey : struct
    {
        ///<inheritdoc/>
        public async Task<T> GetItemFromNodeAsync(JsonElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren)
        {
            var instance = await GetObjectFromNodeAsync(element, typeof(T), activator, repoProvider, persistChildren);
            return (T)instance;
        }

        internal static async Task<object> GetObjectFromNodeAsync(JsonElement element, Type t, Func<Type, object> activator,
            Func<Type, dynamic> repoProvider, bool persistChildren = false)
        {
            // Using the activator so the instance can have dependencies
            var instance = activator(t);

            foreach (var property in t.PersistedProperties(StorageModeOptions.Json))
            {
                if (!element.TryGetProperty(property.GetPropertyName(StorageModeOptions.Json), out var propertyElement)) continue;
                
                if (property.PropertyType.IsString())
                {
                    property.SetValue(instance, propertyElement.GetString(), null);
                }
                else if (property.PropertyType.IsGuid())
                {
                    property.SetValue(instance, propertyElement.GetGuid(), null);
                }
                else if (property.PropertyType.IsPersistentObject())
                {
                    if (persistChildren)
                    {
                        var id = propertyElement.GetGuid();
                        var repository = property.PropertyType.CreateRepository(repoProvider);
                        var persistentObject = await repository.LoadAsync(id).ConfigureAwait(false);
                        property.SetValue(instance, persistentObject, null);
                    }
                    else
                    {
                        var persistentObject =
                            await GetObjectFromNodeAsync(propertyElement, property.PropertyType, activator, repoProvider);
                        property.SetValue(instance, persistentObject, null);
                    }
                }
                else if (property.PropertyType.IsPersistentObjectEnumerable())
                {
                    if (persistChildren)
                    {
                        var persistentObjectIds = propertyElement.EnumerateArray().Select(e => e.GetString());
                        var repository = property.PropertyType.CreateEnumerableRepository(repoProvider);
                        var collection = await repository.LoadCollectionByIdsAsync(persistentObjectIds)
                            .ConfigureAwait(false);
                        property.SetValue(instance, collection, null);
                    }
                    else
                    {
                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var combinedType = typeof(List<>).MakeGenericType(elementType);
                        dynamic collection = activator(combinedType);
                        var children = propertyElement.EnumerateArray();
                        foreach (var childElement in children)
                        {
                            var child =
                                await GetObjectFromNodeAsync(childElement, elementType, activator, repoProvider);
                            collection.Add((dynamic) child);
                        }

                        property.SetValue(instance, collection, null);
                    }
                }
                else
                {
                    property.SetValue(instance,
                        Convert.ChangeType(propertyElement.GetString(), property.PropertyType), null);
                }
            }

            return instance;
        }

        ///<inheritdoc/>
        public T GetItemFromNode(JsonElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren)
        {
            return (T)GetObjectFromNode(element, typeof(T), activator, repoProvider, persistChildren);
        }

        private static object GetObjectFromNode(JsonElement element, Type t, Func<Type, object> activator, Func<Type, dynamic> repoProvider,
            bool persistChildren = false)
        {
            // Using the activator so the instance can have dependencies
            var instance = activator(t);

            foreach (var property in t.PersistedProperties(StorageModeOptions.Json))
            {
                if (!element.TryGetProperty(property.GetPropertyName(StorageModeOptions.Json), out var propertyElement)) continue;

                if (property.PropertyType.IsString())
                {
                    property.SetValue(instance, propertyElement.GetString(), null);
                }
                else if (property.PropertyType.IsGuid())
                {
                    property.SetValue(instance, propertyElement.GetGuid(), null);
                }
                else if (property.PropertyType.IsPersistentObject())
                {
                    if (persistChildren)
                    {
                        var id = propertyElement.GetGuid();
                        var repository = property.PropertyType.CreateRepository(repoProvider);
                        var persistentObject = repository.Load(id);
                        property.SetValue(instance, persistentObject, null);
                    }
                    else
                    {
                        var persistentObject = GetObjectFromNode(
                            propertyElement, 
                            property.PropertyType, 
                            activator, 
                            repoProvider);
                        property.SetValue(instance, persistentObject, null);
                    }
                }
                else if (property.PropertyType.IsPersistentObjectEnumerable())
                {
                    if (persistChildren)
                    {
                        var persistentObjectIds = propertyElement.EnumerateArray().Select(e => e.GetString());
                        var repository = property.PropertyType.CreateEnumerableRepository(repoProvider);
                        var collection = repository.LoadCollectionByIds(persistentObjectIds);
                        property.SetValue(instance, collection, null);
                    }
                    else
                    {
                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var combinedType = typeof(List<>).MakeGenericType(elementType);
                        dynamic collection = activator(combinedType);
                        var children = propertyElement.EnumerateArray();
                        foreach (var childElement in children)
                        {
                            var child = GetObjectFromNode(childElement, elementType, activator, repoProvider);
                            collection.Add((dynamic)child);
                        }
                        property.SetValue(instance, collection, null);
                    }
                }
                else
                {
                    property.SetValue(instance,
                        Convert.ChangeType(propertyElement.GetString(), property.PropertyType), null);
                }
            }

            return instance;
        }
    }
}