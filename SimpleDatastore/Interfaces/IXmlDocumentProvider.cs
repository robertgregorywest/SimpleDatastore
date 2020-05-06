using System.Xml;

namespace SimpleDatastore.Interfaces
{
    public interface IXmlDocumentProvider<T> where T : PersistentObject
    {
        XmlDocument GetDocument();
        void SaveDocument(XmlDocument document);
    }
}
