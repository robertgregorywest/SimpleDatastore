using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDatastore
{
    public interface IRepository<T> where T : PersistentObject
    {
        T Load(Guid id);
        IList<T> LoadList();
        IList<T> LoadListUnsorted();
        bool Save(T instance);
        bool Delete(Guid id);
    }
}
