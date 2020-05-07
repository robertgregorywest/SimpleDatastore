using System;
using System.Xml;

namespace SimpleDatastore.Extensions
{
    internal static class XmlDocumentExtensions
    {
        internal static XmlNode SelectExistingNode(this XmlDocument doc, Guid id)
        {
            return doc.DocumentElement?.SelectSingleNode(
                $"{Constants.DataItemName}[{PersistentObject.Identifier} = \"{id.ToString()}\"]");
        }
    }
}
