using System;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public abstract class ItemResolver<T> where T : PersistentObject
    {
        private readonly IServiceProvider _provider;

        [UsedImplicitly]
        protected ItemResolver(IServiceProvider provider)
        {
            _provider = provider;
        }
        
        protected async Task SetPersistentObjectPropertyAsync(PropertyInfo property, T instance, Guid id)
        {
            var repository = CreateRepository(property);
            var persistentObject = await repository.LoadAsync(id).ConfigureAwait(false);
            property.SetValue(instance, persistentObject, null);
        }
        
        protected void SetPersistentObjectProperty(PropertyInfo property, T instance, Guid id)
        {
            var repository = CreateRepository(property);
            var persistentObject = repository.Load(id);
            property.SetValue(instance, persistentObject, null);
            
        }

        private dynamic CreateRepository(PropertyInfo property)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
            dynamic repository = _provider.GetService(repositoryType);
            return repository;
        }
        
        protected async Task SetPersistentObjectEnumerablePropertyAsync(PropertyInfo property, T instance, string[] persistentObjectIds)
        {
            var repository = CreateEnumerableRepository(property);
            var collection = await repository.LoadCollectionByIdsAsync(persistentObjectIds)
                .ConfigureAwait(false);
            property.SetValue(instance, collection, null);
        }
        
        protected void SetPersistentObjectEnumerableProperty(PropertyInfo property, T instance, string[] persistentObjectIds)
        {
            var repository = CreateEnumerableRepository(property);
            var collection = repository.LoadCollectionByIds(persistentObjectIds);
            property.SetValue(instance, collection, null);
        }
        
        private dynamic CreateEnumerableRepository(PropertyInfo property)
        {
            var elementType = property.PropertyType.GetGenericArguments()[0];
            var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
            dynamic repository = _provider.GetService(repositoryType);
            return repository;
        }
    }
}
