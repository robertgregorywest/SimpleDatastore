using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SimpleDatastore
{
    internal class CollectionMapper<T> where T : PersistentObject
    {
        private readonly IDependencyResolver _dependencyResolver;

        public CollectionMapper(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public IList<T> Map(string[] persistentObjectIds)
        {
            var list = new List<T>();

            foreach (var persistentObjectId in persistentObjectIds)
            {
                var repository = _dependencyResolver.GetService<BaseRepository<T>>();
                var persistentObject = repository.Load(persistentObjectId.ToGuid());
                if (persistentObject != null)
                {
                    list.Add(persistentObject);
                }
            }

            return list;
        }
    }
}
