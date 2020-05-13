using System.Xml;

namespace SimpleDatastore.Interfaces
{
    public interface IDocumentProvider<T> where T : PersistentObject
    {
        XmlDocument GetDocument();
        void SaveDocument(XmlDocument document);
    }
}
