using NSubstitute;
using NUnit.Framework;
using System.Linq;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore.Tests
{
    public class CollectionMapperTests
    {
        [Test]
        public void Map_should_return_list()
        {
            var repository = Substitute.For<IRepository<FakeObject>>();
            repository.Load(FakeObject.InstanceIdentifier).Returns(FakeObject.Instance);

            var ids = new string[] { FakeObject.IdentifierValue};

            var mapper = new CollectionMapper<FakeObject>(repository);

            var result = mapper.Map(ids);

            Assert.AreEqual(result.First(), FakeObject.Instance);
        }

        [Test]
        public void Map_two_items_expect_list_with_two_items()
        {
            var repo = Substitute.For<IRepository<FakeObject>>();
            repo.Load(FakeObject.InstanceIdentifier).Returns(FakeObject.Instance);
            repo.Load(FakeObject.SecondInstanceIdentifier).Returns(FakeObject.SecondInstance);

            var ids = new string[] { FakeObject.IdentifierValue, FakeObject.IdentifierValue2 };

            var mapper = new CollectionMapper<FakeObject>(repo);

            var result = mapper.Map(ids);

            Assert.IsTrue(FakeObject.SortedList.SequenceEqual(result));
        }
    }
}
