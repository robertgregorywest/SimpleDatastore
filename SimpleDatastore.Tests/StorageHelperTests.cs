using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using System.Xml;

namespace SimpleDatastore.Tests
{
    public class StorageHelperTests
    {
        private IItemResolver<FakeObject> _resolver;
        private IDocumentProvider<FakeObject> _provider;

        [SetUp]
        public void Setup()
        {
            _resolver = Substitute.For<IItemResolver<FakeObject>>();
            _provider = Substitute.For<IDocumentProvider<FakeObject>>();
        }

        [TearDown]
        public void Cleanup()
        {
            _resolver = null;
            _provider = null;
        }

        [Test]
        public void SaveObject_should_save_document()
        {
            _provider.GetDocument().Returns(FakeDocuments.EmptyDocument);

            var helper = new StorageHelper<FakeObject>(_resolver, _provider);

            helper.SaveObject(FakeObject.Instance);

            _provider.Received().SaveDocument(Arg.Is<XmlDocument>(d => d.InnerXml == FakeDocuments.SingleFakeObjectDocument.InnerXml));
        }

        [Test]
        public void DeleteObject_should_save_empty_document()
        {
            _provider.GetDocument().Returns(FakeDocuments.SingleFakeObjectDocument);

            var agent = new StorageHelper<FakeObject>(_resolver, _provider);

            agent.DeleteObject(FakeObject.InstanceIdentifier);

            _provider.Received().SaveDocument(Arg.Is<XmlDocument>(d => d.InnerXml == FakeDocuments.EmptyDocument.InnerXml));
        }
    }
}
