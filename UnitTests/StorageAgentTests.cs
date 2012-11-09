using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SimpleDatastore;
using System.Xml;

namespace UnitTests
{
    [TestClass]
    public class StorageAgentTests
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

        [TestMethod]
        public void GetObject_expect_correct_object()
        {
            _storage.Stub(s => s.Get()).Return(FakeDocuments.SingleFakeObjectDocument);

            var agent = new StorageAgent<FakeObject>(_config, _storage);

            var result = agent.GetObject(FakeObject.FakeObjectIdentifier);

            Assert.AreEqual(result, FakeObject.Instance);
        }

        [TestMethod]
        public void GetCollection_expect_correct_collection()
        {
            _storage.Stub(s => s.Get()).Return(FakeDocuments.SingleFakeObjectDocument);

            var agent = new StorageAgent<FakeObject>(_config, _storage);

            var result = agent.GetCollection();

            Assert.AreEqual(result.First(), FakeObject.Instance);
        }

        [TestMethod]
        public void SaveObject_should_save_document()
        {
            _storage.Stub(s => s.Get()).Return(FakeDocuments.EmptyDocument);

            var agent = new StorageAgent<FakeObject>(_config, _storage);

            var result = agent.SaveObject(FakeObject.Instance);

            Assert.IsTrue(result);
            _storage.AssertWasCalled(x => x.Save(Arg<XmlDocument>.Matches(y => y.InnerText.Equals(FakeDocuments.SingleFakeObjectDocument.InnerText))));
        }

        [TestMethod]
        public void DeleteObject_should_save_empty_document()
        {
            _storage.Stub(s => s.Get()).Return(FakeDocuments.SingleFakeObjectDocument);

            var agent = new StorageAgent<FakeObject>(_config, _storage);

            var result = agent.DeleteObject(FakeObject.FakeObjectIdentifier);

            Assert.IsTrue(result);
            _storage.AssertWasCalled(x => x.Save(Arg<XmlDocument>.Matches(y => y.InnerText.Equals(FakeDocuments.EmptyDocument.InnerText))));
        }
    }
}
