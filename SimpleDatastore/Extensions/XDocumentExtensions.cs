using System;
using System.Linq;
using System.Xml.Linq;

namespace SimpleDatastore.Extensions
{
    internal static class XDocumentExtensions
    {
        internal static XElement GetElementById(this XDocument doc, Guid id)
        {
            return doc
                .Descendants(PersistentObject.DataItemName)
                .FirstOrDefault(el => el.Element(PersistentObject.Identifier)?.Value == id.ToString());
        }
    }
}