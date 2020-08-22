using System;
using NSubstitute;
using NUnit.Framework;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class XmlSerializerTests
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
        public void Write_persistChildren_false_with_child_objects_should_persist_ids()
        {
            dynamic RepoProvider(Type t) => _provider;
            var result = XmlSerializer.Write(FakeObject.Instance, RepoProvider, false);
            Assert.AreEqual(result.ToString(), FakeDocuments.SingleFakeObjectXElement.ToString());
        }
    }
}