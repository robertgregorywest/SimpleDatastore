using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDatastore
{
    public interface IStorageAgent<T> where T : PersistentObject
    {
        IList<T> GetCollection();
        T GetObject(Guid id);
        void SaveObject(T instance);
        void DeleteObject(Guid id);
    }
}
