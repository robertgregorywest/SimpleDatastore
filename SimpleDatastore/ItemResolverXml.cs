using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using SimpleDatastore.Interfaces;
using SimpleDatastore.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleDatastore
{
    public class ItemResolverXml<T> : ItemResolver<T>, IItemResolverXml<T> where T : PersistentObject
    {
        private readonly Func<T> _activator;

        [UsedImplicitly]
        public ItemResolverXml(IServiceProvider provider): base(provider)
        {
            _activator = () => ActivatorUtilities.CreateInstance<T>(provider);
        }

        public ItemResolverXml(IServiceProvider provider, Func<T> activator): base(provider)
        {
            _activator = activator;
        }

        ///<inheritdoc/>
        public async Task<T> GetItemFromNodeAsync(XElement element)
        {
            // Using the activator so the instance can have dependencies
            var instance = _activator.Invoke();

            foreach (var property in typeof(T).GetValidProperties())
            {
                var propertyElement = element.Elements(property.GetPropertyName()).First();
                
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
                    await SetPersistentObjectPropertyAsync(property, instance, propertyElement.Value.ToGuid())
                        .ConfigureAwait(false);
                }
                else if (property.PropertyType.IsAPersistentObjectEnumerable())
                {
                    await SetPersistentObjectEnumerablePropertyAsync(property,
                            instance,
                            propertyElement.Value.Split(','))
                        .ConfigureAwait(false);
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
        public T GetItemFromNode(XElement element)
        {
            // Using the activator so the instance can have dependencies
            var instance = _activator.Invoke();

            foreach (var property in typeof(T).GetValidProperties())
            {
                var propertyElement = element.Elements(property.GetPropertyName()).First();
                
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
                    SetPersistentObjectProperty(property, instance, propertyElement.Value.ToGuid());
                }
                else if (property.PropertyType.IsAPersistentObjectEnumerable())
                {
                    SetPersistentObjectEnumerableProperty(property,
                            instance,propertyElement.Value.Split(','));
                }
                else
                {
                    property.SetValue(instance, 
                        Convert.ChangeType(propertyElement.Value, property.PropertyType), null);
                }
            }
            return instance;
        }
    }
}
