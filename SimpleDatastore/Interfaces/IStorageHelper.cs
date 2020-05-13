using System;
using System.Collections.Generic;

namespace SimpleDatastore.Interfaces
{
    public interface IStorageHelper<T> where T : PersistentObject
    {
        IEnumerable<T> GetCollection();
        T GetObject(Guid id);
        void SaveObject(T instance);
        void DeleteObject(Guid id);
    }
}
