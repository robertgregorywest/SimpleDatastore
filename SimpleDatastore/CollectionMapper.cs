using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SimpleDatastore
{
    internal class CollectionMapper<T> where T : PersistentObject
    {
        private readonly IRepository<T> _repository;

        public CollectionMapper(IRepository<T> repository)
        {
            _repository = repository;
        }

        public IList<T> Map(string[] persistentObjectIds)
        {
            var list = new List<T>();

            foreach (var persistentObjectId in persistentObjectIds)
            {
                var persistentObject = _repository.Load(persistentObjectId.ToGuid());
                if (persistentObject != null)
                {
                    list.Add(persistentObject);
                }
            }

            return list;
        }
    }
}
