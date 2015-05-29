using System;
using System.Collections.Generic;

namespace SimpleDatastore
{
    public interface IRepository<T> where T : PersistentObject
    {
        T Load(Guid id);
        IList<T> LoadList();
        IList<T> LoadListUnsorted();
        void Save(T instance);
        void Delete(Guid id);
    }
}
