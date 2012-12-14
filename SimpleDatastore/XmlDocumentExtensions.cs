using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SimpleDatastore
{
    internal static class XmlDocumentExtensions
    {
        internal static XmlNode SelectExistingNode(this XmlDocument doc, Guid id)
        {
            return doc.DocumentElement.SelectSingleNode(string.Format("{0}[{1} = \"{2}\"]", Constants.DataItemName, PersistentObject.Identifier, id.ToString()));
        }
    }
}
