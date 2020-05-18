using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.XPath;
using JetBrains.Annotations;
using SimpleDatastore.Interfaces;
using SimpleDatastore.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleDatastore
{
    public class ItemResolver<T> : IItemResolver<T> where T : PersistentObject
    {
        private readonly IServiceProvider _provider;

        private readonly Func<T> _activator;

        [UsedImplicitly]
        public ItemResolver(IServiceProvider provider)
        {
            _provider = provider;
            _activator = () => ActivatorUtilities.CreateInstance<T>(_provider);
        }

        public ItemResolver(IServiceProvider provider, Func<T> activator)
        {
            _provider = provider;
            _activator = activator;
        }

        public async Task<T> GetItemFromNodeAsync(XPathNavigator nav)
        {
            // Using the activator the instance so that objects can have dependencies
            var instance = _activator.Invoke();

            foreach (var property in typeof(T).GetValidProperties())
            {
                if (!nav.MoveToChild(property.GetPropertyName(), "")) continue;
                
                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(instance, nav.Value, null);
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    property.SetValue(instance, new Guid(nav.Value), null);
                }
                else if (property.PropertyType.IsAPersistentObject())
                {
                    await SetPersistentObjectProperty(property, instance, nav.Value.ToGuid());
                }
                else if (property.PropertyType.IsAPersistentObjectEnumerable())
                {
                    await SetPersistentObjectEnumerableProperty(property, instance, nav.Value.Split(','));
                }
                else
                {
                    property.SetValue(instance, Convert.ChangeType(nav.Value, property.PropertyType), null);
                }

                nav.MoveToParent();
            }
            return instance;
        }

        private async Task SetPersistentObjectProperty(PropertyInfo property, T instance, Guid id)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
            var repository = _provider.GetService(repositoryType);
            if (repository is IRepository iRepository)
            {
                var persistentObject = await iRepository.LoadObjectAsync(id);
                property.SetValue(instance, persistentObject, null);
            }
        }

        private async Task SetPersistentObjectEnumerableProperty(PropertyInfo property, T instance, string[] persistentObjectIds)
        {
            var elementType = property.PropertyType.GetGenericArguments()[0];
            var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
            var repository = _provider.GetService(repositoryType);
            if (repository is IRepository iRepository)
            {
                var collection = await iRepository.LoadObjectCollectionByIdsAsync(persistentObjectIds);
                property.SetValue(instance, collection, null);
            }
        }
    }
}
