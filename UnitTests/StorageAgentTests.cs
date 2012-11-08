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
        private IStorageDocument<TestObject> _storage;

        [TestInitialize]
        public void Setup()
        {
            _config = MockRepository.GenerateStub<IConfiguration>();
            _storage = MockRepository.GenerateStub<IStorageDocument<TestObject>>();

            _config.Stub(c => c.DependencyResolver).Return(new FakeDependencyResolver());
        }

        [TestMethod]
        public void GetObject_expect_correct_object()
        {
            _storage.Stub(s => s.Get()).Return(TestDocuments.SingleTestObjectDocument);

            var agent = new StorageAgent<TestObject>(_config, _storage);

            var result = agent.GetObject(TestDocuments.TestObjectIdentifier);

            Assert.AreEqual(result.Id, TestDocuments.TestObjectIdentifier);
        }

        [TestMethod]
        public void GetCollection_expect_correct_collection()
        {
            _storage.Stub(s => s.Get()).Return(TestDocuments.SingleTestObjectDocument);

            var agent = new StorageAgent<TestObject>(_config, _storage);

            var result = agent.GetCollection();

            Assert.AreEqual(result.First().Id, TestDocuments.TestObjectIdentifier);
        }

        [TestMethod]
        public void SaveObject_should_save_document()
        {
            _storage.Stub(s => s.Get()).Return(TestDocuments.EmptyDocument);

            var agent = new StorageAgent<TestObject>(_config, _storage);

            var testObject = new TestObject() { Id = TestDocuments.TestObjectIdentifier };

            var result = agent.SaveObject(testObject);

            Assert.IsTrue(result);
            _storage.AssertWasCalled(x => x.Save(Arg<XmlDocument>.Matches(y => y.InnerText.Equals(TestDocuments.SingleTestObjectDocument.InnerText))));
        }

        [TestMethod]
        public void DeleteObject_should_save_empty_document()
        {
            _storage.Stub(s => s.Get()).Return(TestDocuments.SingleTestObjectDocument);

            var agent = new StorageAgent<TestObject>(_config, _storage);

            var result = agent.DeleteObject(TestDocuments.TestObjectIdentifier);

            Assert.IsTrue(result);
            _storage.AssertWasCalled(x => x.Save(Arg<XmlDocument>.Matches(y => y.InnerText.Equals(TestDocuments.EmptyDocument.InnerText))));
        }
    }
}
