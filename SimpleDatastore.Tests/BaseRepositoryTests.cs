using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore.Tests
{
    public class BaseRepositoryTests
    {
        private IStorageHelper<FakeObject> _storageHelper;
        private IOptions<SimpleDatastoreOptions> _options;
        private ICache _cache;

        [SetUp]
        public void Setup()
        {
            _storageHelper = Substitute.For<IStorageHelper<FakeObject>>();
            _options = Substitute.For<IOptions<SimpleDatastoreOptions>>();
            _options.Value.Returns(new SimpleDatastoreOptions() { EnableCaching = false });
            _cache = Substitute.For<ICache>();
        }

        [TearDown]
        public void Cleanup()
        {
            _storageHelper = null;
            _options = null;
            _cache = null;
        }

        [Test]
        public void LoadCollection_expect_collection_in_storage_order()
        {
            _storageHelper.GetCollection().Returns(FakeObject.Collection);

            var repo = new BaseRepository<FakeObject>(_storageHelper, _options, _cache);

            var result = repo.LoadCollection();

            Assert.IsTrue(FakeObject.Collection.SequenceEqual(result));
        }

        [Test]
        public void Save_expect_call_StorageHelper_SaveObject()
        {
            var repo = new BaseRepository<FakeObject>(_storageHelper, _options, _cache);

            repo.Save(FakeObject.Instance);

            _storageHelper.Received().SaveObject(FakeObject.Instance);
        }

        [Test]
        public void Save_with_empty_Guid_expect_NewGuid()
        {
            var newInstance = new FakeObject();

            var repo = new BaseRepository<FakeObject>(_storageHelper, _options, _cache);

            repo.Save(newInstance);

            Assert.AreNotEqual(newInstance.Id, Guid.Empty);
        }

        [Test]
        public void Delete_expect_call_helper_delete_object()
        {
            var repo = new BaseRepository<FakeObject>(_storageHelper, _options, _cache);

            repo.Delete(FakeObject.InstanceIdentifier);

            _storageHelper.Received().DeleteObject(FakeObject.InstanceIdentifier);
        }

        [Test]
        public void Delete_expect_cache_purged()
        {
            _options.Value.Returns(new SimpleDatastoreOptions() { EnableCaching = true });

            var repo = new BaseRepository<FakeObject>(_storageHelper, _options, _cache);

            repo.Delete(FakeObject.InstanceIdentifier);

            _cache.Received().Remove(FakeObject.CacheKey);
            _cache.Received().Remove(FakeObject.RootCacheKey);
        }
    }
}
