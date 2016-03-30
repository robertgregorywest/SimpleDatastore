using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SimpleDatastore;
using System.Xml;
using System.Web.Mvc;
using SimpleDatastore.Interfaces;

namespace UnitTests
{
    [TestClass]
    public class StorageHelperTests
    {
        private IDependencyResolver _resolver;
        private IStorageDocument<FakeObject> _storage;

        [TestInitialize]
        public void Setup()
        {
            _resolver = new FakeDependencyResolver();
            _storage = MockRepository.GenerateStub<IStorageDocument<FakeObject>>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _resolver = null;
            _storage = null;
        }

        [TestMethod]
        public void GetObject_expect_correct_object()
        {
            _storage.Stub(s => s.Get()).Return(FakeDocuments.SingleFakeObjectDocument);

            var agent = new StorageHelper<FakeObject>(_resolver, _storage);

            var result = agent.GetObject(FakeObject.InstanceIdentifier);

            Assert.AreEqual(result, FakeObject.Instance);
        }

        [TestMethod]
        public void GetCollection_expect_correct_collection()
        {
            _storage.Stub(s => s.Get()).Return(FakeDocuments.SingleFakeObjectDocument);

            var agent = new StorageHelper<FakeObject>(_resolver, _storage);

            var result = agent.GetCollection();

            Assert.AreEqual(result.First(), FakeObject.Instance);
        }

        [TestMethod]
        public void SaveObject_should_save_document()
        {
            _storage.Stub(s => s.Get()).Return(FakeDocuments.EmptyDocument);

            var agent = new StorageHelper<FakeObject>(_resolver, _storage);

            agent.SaveObject(FakeObject.Instance);

            _storage.AssertWasCalled(s => s.Save(Arg<XmlDocument>.Matches(d => d.InnerText.Equals(FakeDocuments.SingleFakeObjectDocument.InnerText))));
        }

        [TestMethod]
        public void DeleteObject_should_save_empty_document()
        {
            _storage.Stub(s => s.Get()).Return(FakeDocuments.SingleFakeObjectDocument);

            var agent = new StorageHelper<FakeObject>(_resolver, _storage);

            agent.DeleteObject(FakeObject.InstanceIdentifier);

            _storage.AssertWasCalled(x => x.Save(Arg<XmlDocument>.Matches(y => y.InnerText.Equals(FakeDocuments.EmptyDocument.InnerText))));
        }
    }
}
