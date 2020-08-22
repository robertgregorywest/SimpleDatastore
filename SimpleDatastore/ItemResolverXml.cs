using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimpleDatastore.Interfaces;
using SimpleDatastore.Extensions;

namespace SimpleDatastore
{
    public class ItemResolverXml<T> : IItemResolverXml<T> where T : PersistentObject
    {
        ///<inheritdoc/>
        public async Task<T> GetItemFromNodeAsync(XElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren)
        {
            var instance = await GetObjectFromNodeAsync(element, typeof(T), activator, repoProvider, persistChildren);
            return (T)instance;
        }

        internal static async Task<object> GetObjectFromNodeAsync(XElement element, Type t, Func<Type, object> activator,
            Func<Type, dynamic> repoProvider, bool persistChildren = false)
        {
            // Using the activator so the instance can have dependencies
            var instance = activator(t);

            foreach (var property in t.GetValidProperties())
            {
                var propertyElement = element.Elements(property.GetPropertyName()).SingleOrDefault();

                if (propertyElement == null) continue;

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(instance, propertyElement.Value, null);
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    property.SetValue(instance, new Guid(propertyElement.Value), null);
                }
                else if (property.PropertyType.IsAPersistentObject())
                {
                    if (persistChildren)
                    {
                        var id = propertyElement.Value.ToGuid();
                        var repository = CreateRepository(property, repoProvider);
                        var persistentObject = await repository.LoadAsync(id).ConfigureAwait(false);
                        property.SetValue(instance, persistentObject, null);
                    }
                    else
                    {
                        var persistentObject =
                            await GetObjectFromNodeAsync(propertyElement.Element(PersistentObject.DataItemName),
                                property.PropertyType, activator, repoProvider);
                        property.SetValue(instance, persistentObject, null);
                    }
                }
                else if (property.PropertyType.IsAPersistentObjectEnumerable())
                {
                    if (persistChildren)
                    {
                        var persistentObjectIds = propertyElement.Value.Split(',');
                        var repository = CreateEnumerableRepository(property, repoProvider);
                        var collection = await repository.LoadCollectionByIdsAsync(persistentObjectIds)
                            .ConfigureAwait(false);
                        property.SetValue(instance, collection, null);
                    }
                    else
                    {
                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var combinedType = typeof(List<>).MakeGenericType(elementType);
                        dynamic collection = activator(combinedType);
                        var children = propertyElement.Elements(PersistentObject.DataItemName);
                        foreach (var childElement in children)
                        {
                            var child =
                                await GetObjectFromNodeAsync(childElement, elementType, activator, repoProvider);
                            collection.Add((dynamic)child);
                        }
                        property.SetValue(instance, collection, null);
                    }
                }
                else
                {
                    property.SetValue(instance,
                        Convert.ChangeType(propertyElement.Value, property.PropertyType), null);
                }
            }

            return instance;
        }

        ///<inheritdoc/>
        public T GetItemFromNode(XElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren)
        {
            return (T)GetObjectFromNode(element, typeof(T), activator, repoProvider, persistChildren);
        }

        private static object GetObjectFromNode(XElement element, Type t, Func<Type, object> activator, Func<Type, dynamic> repoProvider,
            bool persistChildren = false)
        {
            // Using the activator so the instance can have dependencies
            var instance = activator(t);

            foreach (var property in t.GetValidProperties())
            {
                var propertyElement = element.Elements(property.GetPropertyName()).SingleOrDefault();

                if (propertyElement == null) continue;

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(instance, propertyElement.Value, null);
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    property.SetValue(instance, new Guid(propertyElement.Value), null);
                }
                else if (property.PropertyType.IsAPersistentObject())
                {
                    if (persistChildren)
                    {
                        var id = propertyElement.Value.ToGuid();
                        var repository = CreateRepository(property, repoProvider);
                        var persistentObject = repository.Load(id);
                        property.SetValue(instance, persistentObject, null);
                    }
                    else
                    {
                        var persistentObject =
                            GetObjectFromNode(propertyElement.Element(PersistentObject.DataItemName), property.PropertyType, activator, repoProvider);
                        property.SetValue(instance, persistentObject, null);
                    }
                }
                else if (property.PropertyType.IsAPersistentObjectEnumerable())
                {
                    if (persistChildren)
                    {
                        var persistentObjectIds = propertyElement.Value.Split(',');
                        var repository = CreateEnumerableRepository(property, repoProvider);
                        var collection = repository.LoadCollectionByIds(persistentObjectIds);
                        property.SetValue(instance, collection, null);
                    }
                    else
                    {
                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var combinedType = typeof(List<>).MakeGenericType(elementType);
                        dynamic collection = activator(combinedType);
                        var children = propertyElement.Elements(PersistentObject.DataItemName);
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
                        Convert.ChangeType(propertyElement.Value, property.PropertyType), null);
                }
            }

            return instance;
        }

        private static dynamic CreateRepository(PropertyInfo property, Func<Type, dynamic> repoProvider)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
            return repoProvider(repositoryType);
        }
        
        private static dynamic CreateEnumerableRepository(PropertyInfo property, Func<Type, dynamic> repoProvider)
        {
            var elementType = property.PropertyType.GetGenericArguments()[0];
            var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
            return repoProvider(repositoryType);
        }
    }
}