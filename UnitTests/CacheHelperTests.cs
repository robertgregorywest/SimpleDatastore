using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDatastore;
using Rhino.Mocks;
using SimpleDatastore.Interfaces;

namespace UnitTests
{
    [TestClass]
    public class CacheHelperTests
    {
        private IConfiguration _config;
        private IStorageDocument<FakeObject> _storage;
        private const int CacheDuration = 60;

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

        private static ICache CacheStub
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
        public void GetObject_caching_enabled_expect_correct_cacheKey_retrieved()
        {
            _config.Stub(c => c.Cache).Return(CacheStub);
            _config.EnableCaching = true;

            var helper = new CacheHelper<FakeObject>(_config);

            helper.GetObject(FakeObject.InstanceIdentifier);

            _config.Cache.AssertWasCalled(c => c.Get(FakeObject.CACHE_KEY));
        }

        [TestMethod]
        public void GetCollection_with_caching_enabled_expect_correct_cacheKey_retrieved()
        {
            _config.Stub(c => c.Cache).Return(CacheStub);
            _config.EnableCaching = true;

            var helper = new CacheHelper<FakeObject>(_config);

            helper.GetCollection();

            _config.Cache.AssertWasCalled(c => c.Get(FakeObject.ROOT_CACHE_KEY));
        }
    }
}
