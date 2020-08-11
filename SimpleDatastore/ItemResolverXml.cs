using System;
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
        public async Task<T> GetItemFromNodeAsync(XElement element, Func<T> activator, Func<Type, dynamic> repoProvider, bool persistChildren)
        {
            // Using the activator so the instance can have dependencies
            var instance = activator.Invoke();

            foreach (var property in typeof(T).GetValidProperties())
            {
                var propertyElement = element.Elements(property.GetPropertyName()).First();

                if (propertyElement == null) continue;

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(instance, propertyElement.Value, null);
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
                }
                else if (property.PropertyType.IsAPersistentObjectEnumerable())
                {
                    var persistentObjectIds = propertyElement.Value.Split(',');
                    if (persistChildren)
                    {
                        var repository = CreateEnumerableRepository(property, repoProvider);
                        var collection = await repository.LoadCollectionByIdsAsync(persistentObjectIds)
                            .ConfigureAwait(false);
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
        public T GetItemFromNode(XElement element, Func<T> activator, Func<Type, dynamic> repoProvider, bool persistChildren)
        {
            // Using the activator so the instance can have dependencies
            var instance = activator.Invoke();

            foreach (var property in typeof(T).GetValidProperties())
            {
                var propertyElement = element.Elements(property.GetPropertyName()).First();

                if (propertyElement == null) continue;

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(instance, propertyElement.Value, null);
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
            return repoProvider.Invoke(repositoryType);
        }
        
        private static dynamic CreateEnumerableRepository(PropertyInfo property, Func<Type, dynamic> repoProvider)
        {
            var elementType = property.PropertyType.GetGenericArguments()[0];
            var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
            return repoProvider.Invoke(repositoryType);
        }
    }
}