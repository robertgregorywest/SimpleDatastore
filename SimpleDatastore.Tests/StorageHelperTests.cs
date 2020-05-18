using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using System.Xml;
using Example.Domain;
using SimpleDatastore.Tests.Extensions;

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
        public async Task SaveObject_should_save_document()
        {
            _provider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.EmptyDocument));

            var helper = new StorageHelper<FakeObject>(_resolver, _provider);

            await helper.SaveObjectAsync(FakeObject.Instance);

            await _provider.Received().SaveDocumentAsync(Arg.Is<XmlDocument>(d => d.InnerXml == FakeDocuments.SingleFakeObjectDocument.InnerXml));
        }

        [Test]
        public async Task DeleteObject_should_save_empty_document()
        {
            _provider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.SingleFakeObjectDocument));

            var helper = new StorageHelper<FakeObject>(_resolver, _provider);

            await helper.DeleteObjectAsync(FakeObject.InstanceIdentifier);

            await _provider.Received().SaveDocumentAsync(Arg.Is<XmlDocument>(d => d.InnerXml == FakeDocuments.EmptyDocument.InnerXml));
        }

        [Test]
        public void BuildXml_with_child_objects_should_persist_ids()
        {
            var result = StorageHelper<Widget>.BuildXml(Widgets.SomeWidget);

            var doc = new XmlDocument();
            doc.LoadXml(Widgets.SomeWidgetSingleDocument.GetFixtureXml());
            var expectedInnerXml = doc.DocumentElement.InnerXml;

            Assert.AreEqual(result, expectedInnerXml);
        }
    }
}
