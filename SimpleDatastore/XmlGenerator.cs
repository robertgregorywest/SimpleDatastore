using System.Collections.Generic;
using System.Text;
using System.Xml;
using SimpleDatastore.Extensions;

namespace SimpleDatastore
{
    internal class XmlGenerator<T>
    {
        private readonly T _instance;

        internal XmlGenerator(T instance)
        {
            _instance = instance;
        } 

        internal string Create()
        {
            var objectStringBuilder = new StringBuilder();

            var settings = new XmlWriterSettings() { OmitXmlDeclaration = true };

            using (var writer = XmlWriter.Create(objectStringBuilder, settings))
            {
                writer.WriteStartElement(Constants.DataItemName);

                foreach (var property in typeof(T).GetValidProperties())
                {
                    var attributeName = property.GetPropertyName();

                    if (attributeName == PersistentObject.Identifier)
                    {
                        writer.WriteElementString(attributeName, property.GetValue(_instance, null).ToString());
                    }
                    else
                    {
                        writer.WriteStartElement(attributeName);
                        if (property.PropertyType.IsAPersistentObject())
                        {
                            var persistentObject = property.GetValue(_instance, null) as PersistentObject;
                            if (persistentObject != null) writer.WriteCData(persistentObject.Id.ToString());
                        }
                        else if (property.PropertyType.IsAPersistentObjectList())
                        {
                            var persistentObjectList = property.GetValue(_instance, null) as IEnumerable<PersistentObject>;
                            if (persistentObjectList != null)
                            {
                                var flattenedList = string.Join<PersistentObject>(",", persistentObjectList);
                                writer.WriteCData(flattenedList);
                            }
                        }
                        else
                        {
                            writer.WriteCData(property.GetValue(_instance, null).ToString());
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

            return objectStringBuilder.ToString();
        }
    }
}
