using System;
using System.Collections.Generic;

namespace SimpleDatastore.Interfaces
{
    public interface IRepository<T> where T : PersistentObject
    {
        T Load(Guid id);
        IList<T> LoadList();
        IList<T> LoadListUnsorted();
        IList<T> LoadListByIds(string[] persistentObjectIds);
        void Save(T instance);
        void Delete(Guid id);
    }
}
