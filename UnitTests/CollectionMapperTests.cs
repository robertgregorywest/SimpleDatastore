using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SimpleDatastore;
using SimpleDatastore.Interfaces;

namespace UnitTests
{
    [TestClass]
    public class CollectionMapperTests
    {
        [TestMethod]
        public void Map_should_return_list()
        {
            var repository = MockRepository.GenerateStub<IRepository<FakeObject>>();
            repository.Stub(r => r.Load(FakeObject.InstanceIdentifier)).Return(FakeObject.Instance);

            var ids = new string[] { FakeObject.IdentifierValue};

            var mapper = new CollectionMapper<FakeObject>(repository);

            var result = mapper.Map(ids);

            Assert.AreEqual(result.First(), FakeObject.Instance);
        }

        [TestMethod]
        public void Map_two_items_expect_list_with_two_items()
        {
            var repo = MockRepository.GenerateStub<IRepository<FakeObject>>();
            repo.Stub(r => r.Load(FakeObject.InstanceIdentifier)).Return(FakeObject.Instance);
            repo.Stub(r => r.Load(FakeObject.SecondInstanceIdentifier)).Return(FakeObject.SecondInstance);

            var ids = new string[] { FakeObject.IdentifierValue, FakeObject.IdentifierValue2 };

            var mapper = new CollectionMapper<FakeObject>(repo);

            var result = mapper.Map(ids);

            Assert.IsTrue(FakeObject.SortedList.SequenceEqual(result));
        }
    }
}
