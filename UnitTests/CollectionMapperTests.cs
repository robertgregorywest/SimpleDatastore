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
    public class CollectionMapperTests
    {
        [TestMethod]
        public void Map_should_return_list()
        {
            var repository = MockRepository.GenerateStub<IRepository<FakeObject>>();
            var ids = new string[] { FakeObject.FakeObjectIdentifier.ToString() };

            repository.Stub(r => r.Load(FakeObject.FakeObjectIdentifier)).Return(FakeObject.Instance);

            var mapper = new CollectionMapper<FakeObject>(repository);

            var result = mapper.Map(ids);

            Assert.AreEqual(result.First(), FakeObject.Instance);
        }
    }
}
