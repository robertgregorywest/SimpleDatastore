using System;
using System.Collections.Generic;
using System.Xml;

namespace SimpleDatastore.Interfaces
{
    public interface IStorageHelper<T> where T : PersistentObject
    {
        XmlDocument GetDocument();
        void SaveDocument(XmlDocument document);
        IList<T> GetCollection();
        T GetObject(Guid id);
        void SaveObject(T instance);
        void DeleteObject(Guid id);
    }
}
