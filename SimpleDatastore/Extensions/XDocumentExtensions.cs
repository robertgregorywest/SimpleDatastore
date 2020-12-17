using System;
using System.Linq;
using System.Xml.Linq;

namespace SimpleDatastore.Extensions
{
    internal static class XDocumentExtensions
    {
        internal static XElement GetElementById<TKey>(this XDocument doc, TKey id)
        {
            return doc
                .Descendants(PersistentObject.DataItemName)
                .FirstOrDefault(el => el.Element(PersistentObject.Identifier)?.Value == id.ToString());
        }

        internal static XDocument AddOrUpdate<T, TKey>(this XDocument doc, T instance, XElement element) 
            where T : PersistentObject<TKey> 
            where TKey : struct
        {
            var existingElement = doc.GetElementById(instance.Id);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(element);
            }
            else
            {
                doc.Root?.Add(element);
            }
            return doc;
        }

        internal static XDocument RemoveById<TKey>(this XDocument doc, TKey id)
        {
            doc.GetElementById(id)?.Remove();
            return doc;
        }
    }
}