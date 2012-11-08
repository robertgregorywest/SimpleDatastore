using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDatastore
{
    public interface IStorageAgent<T> where T : PersistentObject
    {
        bool DeleteObject(Guid id);
        IList<T> GetCollection();
        T GetObject(Guid id);
        bool SaveObject(T instance);
    }
}
