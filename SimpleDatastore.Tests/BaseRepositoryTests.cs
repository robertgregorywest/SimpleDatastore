using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore.Tests
{
    public class BaseRepositoryTests
    {
        private IStorageHelper<FakeObject> _storageHelper;
        private IConfiguration _config;
        private ICache _cache;

        [SetUp]
        public void Setup()
        {
            _storageHelper = Substitute.For<IStorageHelper<FakeObject>>();
            _config = Substitute.For<IConfiguration>();
            _cache = Substitute.For<ICache>();
        }

        [TearDown]
        public void Cleanup()
        {
            _storageHelper = null;
            _config = null;
            _cache = null;
        }

        [Test]
        public void LoadListSorted_expect_sorted_list()
        {
            _storageHelper.GetCollection().Returns(FakeObject.UnsortedList);

            var repo = new BaseRepository<FakeObject>(_storageHelper, _config, _cache);

            var result = repo.LoadList();

            Assert.IsTrue(FakeObject.SortedList.SequenceEqual(result));
        }

        [Test]
        public void Save_expect_call_StorageHelper_SaveObject()
        {
            var repo = new BaseRepository<FakeObject>(_storageHelper, _config, _cache);

            repo.Save(FakeObject.Instance);

            _storageHelper.Received().SaveObject(FakeObject.Instance);
        }

        [Test]
        public void Save_with_empty_Guid_expect_NewGuid()
        {
            var newInstance = new FakeObject();

            var repo = new BaseRepository<FakeObject>(_storageHelper, _config, _cache);

            repo.Save(newInstance);

            Assert.AreNotEqual(newInstance.Id, Guid.Empty);
        }

        [Test]
        public void Delete_expect_call_helper_delete_object()
        {
            var repo = new BaseRepository<FakeObject>(_storageHelper, _config, _cache);

            repo.Delete(FakeObject.InstanceIdentifier);

            _storageHelper.Received().DeleteObject(FakeObject.InstanceIdentifier);
        }

        [Test]
        public void Delete_expect_cache_purged()
        {
            _config.EnableCaching = true;

            var repo = new BaseRepository<FakeObject>(_storageHelper, _config, _cache);

            repo.Delete(FakeObject.InstanceIdentifier);

            _cache.Received().Remove(FakeObject.CacheKey);
            _cache.Received().Remove(FakeObject.RootCacheKey);
        }

        //[Test]
        //public void Map_should_return_list()
        //{
        //    var repository = Substitute.For<IRepository<FakeObject>>();
        //    repository.Load(FakeObject.InstanceIdentifier).Returns(FakeObject.Instance);

        //    var ids = new string[] { FakeObject.IdentifierValue};

        //    var mapper = new CollectionMapper<FakeObject>(repository);

        //    var result = mapper.Map(ids);

        //    Assert.AreEqual(result.First(), FakeObject.Instance);
        //}

        //[Test]
        //public void Map_two_items_expect_list_with_two_items()
        //{
        //    var repo = Substitute.For<IRepository<FakeObject>>();
        //    repo.Load(FakeObject.InstanceIdentifier).Returns(FakeObject.Instance);
        //    repo.Load(FakeObject.SecondInstanceIdentifier).Returns(FakeObject.SecondInstance);

        //    var ids = new string[] { FakeObject.IdentifierValue, FakeObject.IdentifierValue2 };

        //    var mapper = new CollectionMapper<FakeObject>(repo);

        //    var result = mapper.Map(ids);

        //    Assert.IsTrue(FakeObject.SortedList.SequenceEqual(result));
        //}
    }
}
