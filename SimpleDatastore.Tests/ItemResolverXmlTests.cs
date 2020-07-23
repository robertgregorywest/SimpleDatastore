using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SimpleDatastore.Tests
{
    public class ItemResolverXmlTests
    {
        private IServiceProvider _provider;

        [SetUp]
        public void Setup()
        {
            _provider = Substitute.For<IServiceProvider>();
        }

        [TearDown]
        public void Cleanup()
        {
            _provider = null;
        }

        [Test]
        public async Task GetItemFromNode_Should_Return_Object()
        {
            var resolver = new ItemResolverXml<FakeObject>(_provider, () => new FakeObject());

            var result = await resolver.GetItemFromNodeAsync(FakeDocuments.SingleFakeObjectXElement);
        
            Assert.AreEqual(FakeObject.Instance, result);
        }
    }
}
