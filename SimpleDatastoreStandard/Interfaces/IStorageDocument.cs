using System.Xml;

namespace SimpleDatastore.Interfaces
{
    public interface IStorageDocument<T> where T : PersistentObject
    {
        XmlDocument Get();
        void Save(XmlDocument document);
    }
}
