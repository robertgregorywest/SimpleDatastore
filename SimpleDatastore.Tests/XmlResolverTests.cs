using NSubstitute;
using NUnit.Framework;
using System;

namespace SimpleDatastore.Tests
{
    public class XmlResolverTests
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
        public void GetItemFromNode_Should_Return_Object()
        {
            var resolver = new XmlResolver<FakeObject>(_provider, () => new FakeObject());

            var result = resolver.GetItemFromNode(FakeDocuments.SingleFakeObjectNavigator);

            Assert.AreEqual(FakeObject.Instance, result);
        }
    }
}
