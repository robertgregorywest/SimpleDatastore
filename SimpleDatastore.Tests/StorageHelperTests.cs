using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using System.Xml.Linq;

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
        public async Task GetObjectAsync_should_send_element_to_provider()
        {
            _provider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.SingeFakeObjectXDocument));
            _resolver.GetItemFromNodeAsync(null).ReturnsForAnyArgs(FakeObject.Instance);
        
            var helper = new StorageHelper<FakeObject>(_resolver, _provider);

            var actual = await helper.GetObjectAsync(FakeObject.InstanceIdentifier);
            
            Assert.IsNotNull(actual);

            await _resolver.Received()
                .GetItemFromNodeAsync(Arg.Is<XElement>(e => e.ToString() == FakeDocuments.SingleFakeObjectXElement.ToString()));
        }

        [Test]
        public async Task SaveObject_should_save_document()
        {
            _provider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.EmptyXDocument));
        
            var helper = new StorageHelper<FakeObject>(_resolver, _provider);
        
            await helper.SaveObjectAsync(FakeObject.Instance);
        
            await _provider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.SingeFakeObjectXDocument.ToString()));
        }
        
        [Test]
        public async Task DeleteObject_should_save_empty_document()
        {
            _provider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.SingeFakeObjectXDocument));
        
            var helper = new StorageHelper<FakeObject>(_resolver, _provider);
        
            await helper.DeleteObjectAsync(FakeObject.InstanceIdentifier);
        
            await _provider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.EmptyXDocument.ToString()));
        }

        [Test]
        public void BuildXml_with_child_objects_should_persist_ids()
        {
            var result = StorageHelper<FakeObject>.BuildXml(FakeObject.Instance);
            Assert.AreEqual(result.ToString(), FakeDocuments.SingleFakeObjectXElement.ToString());
        }

        [Test]
        public void GetElementById_should_return_Element()
        {
            var actual = StorageHelper<FakeObject>.GetElementById(FakeDocuments.SingeFakeObjectXDocument,
                FakeObject.InstanceIdentifier);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(FakeDocuments.SingleFakeObjectXElement.ToString(), actual.ToString());
        }
}
}
