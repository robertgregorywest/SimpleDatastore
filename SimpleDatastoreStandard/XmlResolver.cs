using System;
using System.Linq;
using System.Reflection;
using System.Xml.XPath;
using SimpleDatastore.Interfaces;
using SimpleDatastore.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleDatastore
{
    public class XmlResolver<T> : IXmlResolver<T> where T : PersistentObject
    {
        private readonly IServiceProvider _provider;

        private readonly Func<Type, object> _activator;

        public XmlResolver(IServiceProvider provider) : this(provider, t => ActivatorUtilities.CreateInstance(provider, t.GetType())) { }

        public XmlResolver(IServiceProvider provider, Func<Type, object> activator)
        {
            _provider = provider;
            _activator = activator;
        }

        public T GetItemFromNode(XPathNavigator nav)
        {
            // Using the provider to activate the instance so that objects can have dependencies
            T instance = (T)_activator.Invoke(typeof(T));

            foreach (var property in typeof(T).GetValidProperties().Where(property => nav.MoveToChild(property.GetPropertyName(), "")))
            {
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
                    SetPersistentObjectProperty(property, instance, nav.Value.ToGuid());
                }
                else if (property.PropertyType.IsAPersistentObjectList())
                {
                    SetPersistentObjectListProperty(property, instance, nav.Value.Split(','));
                }
                else
                {
                    property.SetValue(instance, Convert.ChangeType(nav.Value, property.PropertyType), null);
                }

                nav.MoveToParent();
            }

            return instance;
        }

        private void SetPersistentObjectProperty(PropertyInfo property, T instance, Guid id)
        {
            var repositoryType = typeof(BaseRepository<>).MakeGenericType(property.PropertyType);

            dynamic repository = _activator.Invoke(repositoryType);
            var persistentObject = repository.Load(id);
            property.SetValue(instance, persistentObject, null);
        }

        private void SetPersistentObjectListProperty(PropertyInfo property, T instance, string[] persistentObjectIds)
        {
            var elementType = property.PropertyType.GetGenericArguments()[0];
            var mapperType = typeof(CollectionMapper<>).MakeGenericType(elementType);

            dynamic mapper = _activator.Invoke(mapperType);
            var list = mapper.Map(persistentObjectIds);
            property.SetValue(instance, list, null);
        }
    }
}
