﻿using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore.Tests
{
    public class RepositoryTests
    {
        private IPersistentObjectProvider<FakeObject, Guid> _storageHelper;
        private IOptions<SimpleDatastoreOptions> _options;
        private IMemoryCache _cache;

        [SetUp]
        public void Setup()
        {
            _storageHelper = Substitute.For<IPersistentObjectProvider<FakeObject, Guid>>();
            _options = Substitute.For<IOptions<SimpleDatastoreOptions>>();
            _options.Value.Returns(new SimpleDatastoreOptions() { EnableCaching = false });
            _cache = Substitute.For<IMemoryCache>();
        }

        [TearDown]
        public void Cleanup()
        {
            _storageHelper = null;
            _options = null;
            _cache = null;
        }

        [Test]
        public async Task LoadCollection_expect_collection_in_storage_order()
        {
            _storageHelper.GetCollectionAsync().Returns(Task.FromResult(FakeObject.Collection));

            var repo = new Repository<FakeObject, Guid>(_storageHelper, _cache, _options);

            var result = await repo.LoadCollectionAsync();

            Assert.IsTrue(FakeObject.Collection.SequenceEqual(result));
        }

        [Test]
        public async Task SaveAsync_expect_call_StorageHelper_SaveObjectAsync()
        {
            var repo = new Repository<FakeObject, Guid>(_storageHelper, _cache, _options);

            await repo.SaveAsync(FakeObject.Instance);

            await _storageHelper.Received().SaveObjectAsync(Arg.Is<FakeObject>(f => f.Id == FakeObject.InstanceIdentifier));
        }
        
        [Test]
        public void Save_expect_call_StorageHelper_SaveObject()
        {
            var repo = new Repository<FakeObject, Guid>(_storageHelper, _cache, _options);

            repo.Save(FakeObject.Instance);

            _storageHelper.Received().SaveObject(Arg.Is<FakeObject>(f => f.Id == FakeObject.InstanceIdentifier));
        }

        [Test]
        public async Task DeleteAsync_expect_call_helper_delete_object()
        {
            var repo = new Repository<FakeObject, Guid>(_storageHelper, _cache, _options);

            await repo.DeleteAsync(FakeObject.InstanceIdentifier);

            await _storageHelper.Received().DeleteObjectAsync(FakeObject.InstanceIdentifier);
        }

        [Test]
        public async Task DeleteAsync_expect_cache_purged()
        {
            _options.Value.Returns(new SimpleDatastoreOptions() { EnableCaching = true });

            var repo = new Repository<FakeObject, Guid>(_storageHelper, _cache, _options);

            await repo.DeleteAsync(FakeObject.InstanceIdentifier);

            _cache.Received().Remove(FakeObject.CacheKey);
            _cache.Received().Remove(FakeObject.RootCacheKey);
        }
    }
}
