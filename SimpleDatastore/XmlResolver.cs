﻿using System;
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

        private readonly Func<T> _activator;

        public XmlResolver(IServiceProvider provider)
        {
            _provider = provider;
            _activator = () => ActivatorUtilities.CreateInstance<T>(_provider);
        }

        public XmlResolver(IServiceProvider provider, Func<T> activator)
        {
            _provider = provider;
            _activator = activator;
        }

        public T GetItemFromNode(XPathNavigator nav)
        {
            // Using the activator the instance so that objects can have dependencies
            T instance = _activator.Invoke();

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
                    SetPersistentObjectProperty(property, ref instance, nav.Value.ToGuid());
                }
                else if (property.PropertyType.IsAPersistentObjectList())
                {
                    SetPersistentObjectListProperty(property, ref instance, nav.Value.Split(','));
                }
                else
                {
                    property.SetValue(instance, Convert.ChangeType(nav.Value, property.PropertyType), null);
                }

                nav.MoveToParent();
            }

            return instance;
        }

        private void SetPersistentObjectProperty(PropertyInfo property, ref T instance, Guid id)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
            var repository = _provider.GetService(repositoryType);
            if (repository is IRepository iRepository)
            {
                var persistentObject = iRepository.LoadObject(id);
                property.SetValue(instance, persistentObject, null);
            }
        }

        private void SetPersistentObjectListProperty(PropertyInfo property, ref T instance, string[] persistentObjectIds)
        {
            var elementType = property.PropertyType.GetGenericArguments()[0];
            var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
            var repository = _provider.GetService(repositoryType);
            if (repository is IRepository iRepository)
            {
                var list = iRepository.LoadObjectListByIds(persistentObjectIds);
                property.SetValue(instance, list, null);
            }
        }
    }
}
