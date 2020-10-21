using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    internal static class PersistentObjectConverterXml
    {
        internal static XElement Write(object instance, Func<Type, dynamic> repoProvider, bool persistChildren = false)
        {
            var element = new XElement(PersistentObject.DataItemName);

            foreach (var property in instance.GetType().PersistedProperties())
            {
                var attributeName = property.GetPropertyName();
                var value = property.GetValue(instance, null);

                if (attributeName == PersistentObject.Identifier)
                {
                    element.Add(new XElement(attributeName, value.ToString()));
                    continue;
                }

                if (property.PropertyType.IsPersistentObject() && value is PersistentObject persistentObject)
                {
                    if (persistChildren)
                    {
                        var repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
                        var repository = repoProvider(repositoryType);
                        repository.Save((dynamic) persistentObject);
                        element.Add(new XElement(attributeName, persistentObject.Id.ToString()));
                    }
                    else
                    {
                     element.Add(new XElement(attributeName, Write(persistentObject, repoProvider)));   
                    }
                    continue;
                }

                if (property.PropertyType.IsPersistentObjectEnumerable() &&
                    value is IEnumerable<PersistentObject> persistentObjectEnumerable)
                {
                    var list = persistentObjectEnumerable.ToList();

                    if (persistChildren)
                    {
                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
                        var repository = repoProvider(repositoryType);

                        foreach (var item in list)
                        {
                            repository.Save((dynamic) item);
                        }
                        var flattenedEnumerable = string.Join(",", list);
                        element.Add(new XElement(attributeName, flattenedEnumerable));
                    }
                    else
                    {
                        element.Add(new XElement(attributeName, list.Select(o => Write(o, repoProvider)).ToList()));
                    }
                    continue;
                }

                element.Add(new XElement(attributeName, new XCData(property.GetValue(instance, null).ToString())));
            }

            return element;
        }
    }
}