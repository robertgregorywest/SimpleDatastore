using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDatastore;
using Rhino.Mocks;

namespace UnitTests
{
    [TestClass]
    public class BaseRepositoryTests
    {
        private IConfiguration _config;
        private IStorageDocument<FakeObject> _storage;
        private const int CACHE_DURATION = 60;

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

        private ICache CacheStub
        {
            get
            {
                var cache = MockRepository.GenerateStub<ICache>();
                cache.Stub(c => c.Get(FakeObject.CACHE_KEY)).Return(FakeObject.Instance);
                cache.Stub(c => c.Get(FakeObject.ROOT_CACHE_KEY)).Return(FakeObject.UnsortedList);
                cache.Stub(c => c.PurgeCacheItems(FakeObject.ROOT_CACHE_KEY)).IgnoreArguments();
                return cache;
            }
        }

        [TestMethod]
        public void LoadWithCachingEnabledExpectCorrectCacheKeyRetrieved()
        {
            _config.Stub(c => c.Cache).Return(CacheStub);
            _config.EnableCaching = true;

            var repo = new BaseRepository<FakeObject>(_config);

            var result = repo.Load(FakeObject.InstanceIdentifier);

            _config.Cache.AssertWasCalled(c => c.Get(FakeObject.CACHE_KEY));
        }

        [TestMethod]
        public void LoadWithCachingEnabledExpectHelperNotCalled()
        {
            _config.Stub(c => c.Cache).Return(CacheStub);
            _config.EnableCaching = true;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            var result = repo.Load(FakeObject.InstanceIdentifier);

            helper.AssertWasNotCalled(h => h.GetObject(FakeObject.InstanceIdentifier));
        }

        [TestMethod]
        public void LoadWithCachingEnabledObjectNotInCacheExpectRetrievedObjectToBeCached()
        {            
            var cache = MockRepository.GenerateStub<ICache>();
            cache.Stub(c => c.Get(FakeObject.CACHE_KEY)).Return(null);
            cache.Stub(c => c.CacheData(FakeObject.CACHE_KEY, FakeObject.Instance, CACHE_DURATION));

            _config.Stub(c => c.Cache).Return(cache);
            _config.EnableCaching = true;
            _config.CacheDuration = CACHE_DURATION;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.GetObject(FakeObject.InstanceIdentifier)).Return(FakeObject.Instance);

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            var result = repo.Load(FakeObject.InstanceIdentifier);

            _config.Cache.AssertWasCalled(c => c.CacheData(FakeObject.CACHE_KEY, FakeObject.Instance, CACHE_DURATION));
        }

        [TestMethod]
        public void LoadWithCachingDisabledExpectRetrievedObjectToBeCached()
        {
            _config.EnableCaching = false;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.GetObject(FakeObject.InstanceIdentifier)).Return(FakeObject.Instance);

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            var result = repo.Load(FakeObject.InstanceIdentifier);

            helper.AssertWasCalled(h => h.GetObject(FakeObject.InstanceIdentifier));
        }

        [TestMethod]
        public void LoadWithCachingEnabledObjectNotInCacheExpectObjectToBeRetrieved()
        {
            var cache = MockRepository.GenerateStub<ICache>();
            cache.Stub(c => c.Get(FakeObject.CACHE_KEY)).Return(null);

            _config.Stub(c => c.Cache).Return(cache);
            _config.EnableCaching = true;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.GetCollection()).Return(FakeObject.UnsortedList);

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            var result = repo.LoadListUnsorted();

            helper.AssertWasCalled(h => h.GetCollection());
        }

        [TestMethod]
        public void LoadListUnsortedWithCachingEnabledExpectCorrectCacheKeyRetrieved()
        {
            _config.Stub(c => c.Cache).Return(CacheStub);
            _config.EnableCaching = true;

            var repo = new BaseRepository<FakeObject>(_config);

            var result = repo.LoadListUnsorted();

            _config.Cache.AssertWasCalled(c => c.Get(FakeObject.ROOT_CACHE_KEY));
        }

        [TestMethod]
        public void LoadListUnsortedWithCachingEnabledExpectHelperNotCalled()
        {
            _config.Stub(c => c.Cache).Return(CacheStub);
            _config.EnableCaching = true;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            var result = repo.LoadListUnsorted();

            helper.AssertWasNotCalled(h => h.GetCollection());
        }

        [TestMethod]
        public void LoadListUnsortedWithCachingEnabledObjectNotInCacheExpectObjectToBeRetrieved()
        {
            var cache = MockRepository.GenerateStub<ICache>();
            cache.Stub(c => c.Get(FakeObject.ROOT_CACHE_KEY)).Return(null);

            _config.Stub(c => c.Cache).Return(cache);
            _config.EnableCaching = true;
            _config.CacheDuration = CACHE_DURATION;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.GetCollection()).Return(FakeObject.UnsortedList);

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            var result = repo.LoadListUnsorted();

            helper.AssertWasCalled(h => h.GetCollection());
        }

        [TestMethod]
        public void LoadListUnsortedWithCachingDisabledExpectObjectToBeRetrieved()
        {
            _config.EnableCaching = false;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.GetCollection()).Return(FakeObject.UnsortedList);

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            var result = repo.LoadListUnsorted();

            helper.AssertWasCalled(h => h.GetCollection());
        }

        [TestMethod]
        public void LoadListUnsortedWithCachingEnabledObjectNotInCacheExpectRetrievedObjectToBeCached()
        {
            var cache = MockRepository.GenerateStub<ICache>();
            cache.Stub(c => c.Get(FakeObject.ROOT_CACHE_KEY)).Return(null);
            cache.Stub(c => c.CacheData(FakeObject.ROOT_CACHE_KEY, FakeObject.UnsortedList, CACHE_DURATION));

            _config.Stub(c => c.Cache).Return(cache);
            _config.EnableCaching = true;
            _config.CacheDuration = CACHE_DURATION;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.GetCollection()).Return(FakeObject.UnsortedList);

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            var result = repo.LoadListUnsorted();

            _config.Cache.AssertWasCalled(c => c.CacheData(FakeObject.ROOT_CACHE_KEY, FakeObject.UnsortedList, CACHE_DURATION));
        }

        [TestMethod]
        public void LoadListSortedExpectSortedList()
        {
            _config.EnableCaching = false;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.GetCollection()).Return(FakeObject.UnsortedList);

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            var result = repo.LoadList();

            Assert.IsTrue(FakeObject.SortedList.SequenceEqual(result));
        }

        [TestMethod]
        public void SaveExpectCallHelperSaveObject()
        {
            _config.EnableCaching = false;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.SaveObject(null));

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            repo.Save(FakeObject.Instance);

            helper.AssertWasCalled(h => h.SaveObject(FakeObject.Instance));
        }

        [TestMethod]
        public void SaveWithEmptyGuidExpectNewGuid()
        {
            var newInstance = new FakeObject();

            _config.EnableCaching = false;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.SaveObject(null));

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            repo.Save(newInstance);

            Assert.AreNotEqual(newInstance.Id, Guid.Empty);
        }

        [TestMethod]
        public void SaveWithCachingEnabledExpectCachePurged()
        {
            var cache = CacheStub;

            _config.Stub(c => c.Cache).Return(cache);
            _config.EnableCaching = true;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.SaveObject(null));

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            repo.Save(FakeObject.Instance);

            cache.AssertWasCalled(c => c.PurgeCacheItems(FakeObject.ROOT_CACHE_KEY));
        }

        [TestMethod]
        public void DeleteExpectCallHelperDeleteObject()
        {
            _config.EnableCaching = false;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.DeleteObject(FakeObject.InstanceIdentifier));

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            repo.Delete(FakeObject.InstanceIdentifier);

            helper.AssertWasCalled(h => h.DeleteObject(FakeObject.InstanceIdentifier));
        }

        [TestMethod]
        public void DeleteWithCachingEnabledExpectCachePurged()
        {
            var cache = MockRepository.GenerateStub<ICache>();
            cache.Stub(c => c.PurgeCacheItems(FakeObject.ROOT_CACHE_KEY));

            _config.Stub(c => c.Cache).Return(cache);
            _config.EnableCaching = true;

            var helper = MockRepository.GenerateStub<IStorageAgent<FakeObject>>();
            helper.Stub(h => h.DeleteObject(FakeObject.InstanceIdentifier));

            var repo = new BaseRepository<FakeObject>(_config) { Helper = helper };

            repo.Delete(FakeObject.InstanceIdentifier);

            cache.AssertWasCalled(c => c.PurgeCacheItems(FakeObject.ROOT_CACHE_KEY));
        }
    }
}
