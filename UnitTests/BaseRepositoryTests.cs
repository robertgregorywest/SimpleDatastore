using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDatastore;
using Rhino.Mocks;
using SimpleDatastore.Interfaces;

namespace UnitTests
{
    [TestClass]
    public class BaseRepositoryTests
    {
        private IConfiguration _config;
        private IStorageDocument<FakeObject> _storage;

        [TestInitialize]
        public void Setup()
        {
            _config = MockRepository.GenerateStub<IConfiguration>();
            _storage = MockRepository.GenerateStub<IStorageDocument<FakeObject>>();
            _config.Stub(c => c.DependencyResolver).Return(new FakeDependencyResolver());
        }

        [TestCleanup]
        public void Cleanup()
        {
            _config = null;
            _storage = null;
        }

        [TestMethod]
        public void Load_with_caching_enabled_expect_StorageHelper_not_called()
        {
            _config.EnableCaching = true;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();

            cacheHelper.Stub(c => c.GetObject(FakeObject.InstanceIdentifier)).Return(FakeObject.Instance);

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            var result = repo.Load(FakeObject.InstanceIdentifier);

            storageHelper.AssertWasNotCalled(h => h.GetObject(FakeObject.InstanceIdentifier));
        }

        [TestMethod]
        public void Load_with_caching_enabled_object_not_in_cache_expect_object_to_be_retrieved()
        {
            _config.EnableCaching = true;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.GetCollection()).Return(FakeObject.UnsortedList);

            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();
            cacheHelper.Stub(c => c.GetObject(FakeObject.InstanceIdentifier)).Return(null);

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            var result = repo.LoadListUnsorted();

            storageHelper.AssertWasCalled(h => h.GetCollection());
        }

        [TestMethod]
        public void Load_with_caching_enabled_object_not_in_cache_expect_retrieved_object_to_be_cached()
        {            
            _config.EnableCaching = true;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.GetObject(FakeObject.InstanceIdentifier)).Return(FakeObject.Instance);

            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();
            cacheHelper.Stub(c=> c.GetObject(FakeObject.InstanceIdentifier)).Return(null);
            
            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            var result = repo.Load(FakeObject.InstanceIdentifier);

            cacheHelper.AssertWasCalled(c => c.CacheObject(FakeObject.Instance));
        }

        [TestMethod]
        public void LoadListUnsorted_with_caching_enabled_expect_StorageHelper_not_called()
        {
            _config.EnableCaching = true;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();

            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();
            cacheHelper.Stub(c => c.GetCollection()).Return(FakeObject.UnsortedList);

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            var result = repo.LoadListUnsorted();

            storageHelper.AssertWasNotCalled(h => h.GetCollection());
        }

        [TestMethod]
        public void LoadListUnsorted_with_caching_enabled_object_not_in_cache_expect_object_to_be_retrieved()
        {
            _config.EnableCaching = true;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.GetCollection()).Return(FakeObject.UnsortedList);

            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();
            cacheHelper.Stub(c => c.GetCollection()).Return(null);

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            var result = repo.LoadListUnsorted();

            storageHelper.AssertWasCalled(h => h.GetCollection());
        }

        [TestMethod]
        public void LoadListUnsorted_with_caching_disabled_expect_object_to_be_retrieved()
        {
            _config.EnableCaching = false;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.GetCollection()).Return(FakeObject.UnsortedList);

            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            var result = repo.LoadListUnsorted();

            storageHelper.AssertWasCalled(h => h.GetCollection());
        }

        [TestMethod]
        public void LoadListUnsorted_with_caching_enabled_object_not_in_cache_expect_retrieved_object_to_be_cached()
        {
            _config.EnableCaching = true;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.GetCollection()).Return(FakeObject.UnsortedList);

            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();
            cacheHelper.Stub(c => c.GetCollection()).Return(null);

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            var result = repo.LoadListUnsorted();

            cacheHelper.AssertWasCalled(c => c.CacheCollection(FakeObject.UnsortedList));
        }

        [TestMethod]
        public void LoadListSorted_expect_sorted_list()
        {
            _config.EnableCaching = false;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();

            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();
            cacheHelper.Stub(c => c.GetCollection()).Return(FakeObject.UnsortedList);

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            var result = repo.LoadList();

            Assert.IsTrue(FakeObject.SortedList.SequenceEqual(result));
        }

        [TestMethod]
        public void Save_expect_call_StorageHelper_SaveObject()
        {
            _config.EnableCaching = false;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.SaveObject(null));

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper };

            repo.Save(FakeObject.Instance);

            storageHelper.AssertWasCalled(h => h.SaveObject(FakeObject.Instance));
        }

        [TestMethod]
        public void Save_with_empty_Guid_expect_NewGuid()
        {
            var newInstance = new FakeObject();

            _config.EnableCaching = false;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.SaveObject(null));

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper };

            repo.Save(newInstance);

            Assert.AreNotEqual(newInstance.Id, Guid.Empty);
        }

        [TestMethod]
        public void Save_with_caching_enabled_expect_CachePurged()
        {
            _config.EnableCaching = true;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.SaveObject(FakeObject.Instance));

            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            repo.Save(FakeObject.Instance);

            cacheHelper.AssertWasCalled(c => c.PurgeCacheItems());
        }

        [TestMethod]
        public void Delete_expect_call_helper_delete_object()
        {
            _config.EnableCaching = false;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.DeleteObject(FakeObject.InstanceIdentifier));

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper };

            repo.Delete(FakeObject.InstanceIdentifier);

            storageHelper.AssertWasCalled(h => h.DeleteObject(FakeObject.InstanceIdentifier));
        }

        [TestMethod]
        public void Delete_with_caching_enabled_expect_cache_purged()
        {
            _config.EnableCaching = true;

            var storageHelper = MockRepository.GenerateStub<IStorageHelper<FakeObject>>();
            storageHelper.Stub(h => h.DeleteObject(FakeObject.InstanceIdentifier));

            var cacheHelper = MockRepository.GenerateStub<ICacheHelper<FakeObject>>();

            var repo = new BaseRepository<FakeObject>(_config) { StorageHelper = storageHelper, CacheHelper = cacheHelper };

            repo.Delete(FakeObject.InstanceIdentifier);

            cacheHelper.AssertWasCalled(c => c.PurgeCacheItems());
        }
    }
}
