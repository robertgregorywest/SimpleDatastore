using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace SimpleDatastore.Tests
{
    public class ItemResolverXmlTests
    {
        private IServiceProvider _provider;
        private IOptions<SimpleDatastoreOptions> _options;

        [SetUp]
        public void Setup()
        {
            _provider = Substitute.For<IServiceProvider>();
            _options = Substitute.For<IOptions<SimpleDatastoreOptions>>();
        }

        [TearDown]
        public void Cleanup()
        {
            _provider = null;
            _options = null;
        }

        [Test]
        public async Task GetItemFromNode_Should_Return_Object()
        {
            _options.Value.PersistChildren = true;
            var resolver = new ItemResolverXml<FakeObject>(_provider, _options);

            var result = await resolver.GetItemFromNodeAsync(FakeDocuments.SingleFakeObjectXElement);
        
            Assert.AreEqual(FakeObject.Instance, result);
        }
    }
}
