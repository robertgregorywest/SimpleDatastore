using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDatastore
{
    public interface ICacheHelper<T> where T: PersistentObject
    {
        T GetObject(Guid id);
        IList<T> GetCollection();
        void CacheObject(T instance);        
        void CacheCollection(IList<T> collection);
        void PurgeCacheItems();
    }
}
