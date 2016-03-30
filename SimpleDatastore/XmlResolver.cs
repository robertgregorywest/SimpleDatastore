﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.XPath;
using SimpleDatastore.Extensions;

namespace SimpleDatastore
{
    internal class XmlResolver<T>
    {
        private readonly IDependencyResolver _resolver;

        internal XmlResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        internal T GetItemFromNode(XPathNavigator nav)
        {
            // Using the resolver so that objects can have dependencies
            var instance = _resolver.GetService<T>();

            foreach (var property in typeof(T).GetValidProperties().Where(property => nav.MoveToChild(property.GetPropertyName(), "")))
            {
                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(instance, nav.Value, null);
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    var guid = new Guid(nav.Value);
                    property.SetValue(instance, guid, null);
                }
                else if (property.PropertyType.IsAPersistentObject())
                {
                    var repositoryType = typeof(BaseRepository<>).MakeGenericType(property.PropertyType);
                    dynamic repository = _resolver.GetService(repositoryType);
                    var persistentObject = repository.Load(nav.Value.ToGuid());
                    property.SetValue(instance, persistentObject, null);
                }
                else if (property.PropertyType.IsAPersistentObjectList())
                {
                    var persistentObjectIds = nav.Value.Split(',');
                    var elementType = property.PropertyType.GetGenericArguments()[0];
                    var mapperType = typeof(CollectionMapper<>).MakeGenericType(elementType);
                    dynamic mapper = _resolver.GetService(mapperType);
                    var list = mapper.Map(persistentObjectIds);
                    property.SetValue(instance, list, null);
                }
                else
                {
                    property.SetValue(instance, Convert.ChangeType(nav.Value, property.PropertyType), null);
                }
                nav.MoveToParent();
            }
            return instance;
        }
    }
}
