using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SimpleDatastore;

namespace UnitTests
{
    [TestClass]
    public class StorageAgentTests
    {
        [TestMethod]
        public void GetObject_Expect_Correct_Object()
        {
            var config = MockRepository.GenerateStub<IConfiguration>();
            var storage = MockRepository.GenerateStub<IStorageDocument<TestObject>>();

            config.Stub(c => c.DependencyResolver).Return(new FakeDependencyResolver());
            storage.Stub(s => s.Get()).Return(TestDocuments.SingleTestObject);

            var agent = new StorageAgent<TestObject>(config, storage);

            var result = agent.GetObject(TestDocuments.TestObjectIdentifier);

            Assert.AreEqual(result.Id, TestDocuments.TestObjectIdentifier);
        }
    }
}
