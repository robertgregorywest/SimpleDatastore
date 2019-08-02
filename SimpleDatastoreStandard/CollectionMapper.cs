using System.Collections.Generic;
using System.Linq;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

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
            return persistentObjectIds
                .Select(persistentObjectId => _repository.Load(persistentObjectId.ToGuid()))
                .Where(persistentObject => persistentObject != null)
                .ToList();
        }
    }
}
