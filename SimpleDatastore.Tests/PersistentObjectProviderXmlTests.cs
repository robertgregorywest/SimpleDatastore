using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace SimpleDatastore.Tests
{
    public class PersistentObjectProviderXmlTests
    {
        private IItemResolver<FakeObject, Guid, XElement> _resolver;
        private IDocumentProvider<FakeObject, Guid, XDocument> _documentProvider;
        private IServiceProvider _serviceProvider;
        private IOptions<SimpleDatastoreOptions> _options;

        [SetUp]
        public void Setup()
        {
            _resolver = Substitute.For<IItemResolver<FakeObject, Guid, XElement>>();
            _documentProvider = Substitute.For<IDocumentProvider<FakeObject, Guid, XDocument>>();
            _serviceProvider = Substitute.For<IServiceProvider>();
            _options = Substitute.For<IOptions<SimpleDatastoreOptions>>();

            _options.Value.Returns(new SimpleDatastoreOptions() { PersistChildren = false });
        }

        [TearDown]
        public void Cleanup()
        {
            _resolver = null;
            _documentProvider = null;
            _serviceProvider = null;
            _options = null;
        }

        [Test]
        public async Task GetCollectionAsync_should_return_collection()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.CollectionFakeObjectXDocument));
            _resolver = new ItemResolverXml<FakeObject, Guid, XElement>();
        
            var provider = new PersistentObjectProviderXml<FakeObject, Guid>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = await provider.GetCollectionAsync();
            
            actual.Should()
                .NotBeEmpty()
                .And.HaveCount(c => c == 2)
                .And.BeEquivalentTo(FakeObject.SecondInstance, FakeObject.Instance);
        }
        
        [Test]
        public async Task GetCollectionAsync_empty_document_should_return_empty_collection()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.EmptyXDocument));
            _resolver = new ItemResolverXml<FakeObject, Guid, XElement>();
        
            var provider = new PersistentObjectProviderXml<FakeObject, Guid>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = await provider.GetCollectionAsync();
            
            actual.Should()
                .NotBeNull()
                .And.BeEmpty();
        }
        
        [Test]
        public void GetCollection_empty_document_should_return_empty_collection()
        {
            _documentProvider.GetDocument().Returns(FakeDocuments.EmptyXDocument);
            _resolver = new ItemResolverXml<FakeObject, Guid, XElement>();
        
            var provider = new PersistentObjectProviderXml<FakeObject, Guid>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = provider.GetCollection();
            
            actual.Should()
                .NotBeNull()
                .And.BeEmpty();
        }

        [Test]
        public async Task SaveObject_should_save_document_with_empty_document()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.EmptyXDocument));
        
            var provider = new PersistentObjectProviderXml<FakeObject, Guid>(_resolver, _documentProvider, _serviceProvider, _options);
        
            await provider.SaveObjectAsync(FakeObject.Instance);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.SingeFakeObjectXDocument.ToString()));
        }
        
        [Test]
        public async Task SaveObject_should_update_existing_object()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.CollectionFakeObjectXDocument));
            _resolver = new ItemResolverXml<FakeObject, Guid, XElement>();
        
            var provider = new PersistentObjectProviderXml<FakeObject, Guid>(_resolver, _documentProvider, _serviceProvider, _options);
            
            var actual = FakeObject.SecondInstance;
            actual.Name = FakeObject.NameValue2Updated;
        
            await provider.SaveObjectAsync(actual);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.CollectionFakeObjectXDocumentUpdated.ToString()));
        }
        
        [Test]
        public async Task DeleteObject_should_save_empty_document()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.SingeFakeObjectXDocument));
        
            var provider = new PersistentObjectProviderXml<FakeObject, Guid>(_resolver, _documentProvider, _serviceProvider, _options);
        
            await provider.DeleteObjectAsync(FakeObject.InstanceIdentifier);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.EmptyXDocument.ToString()));
        }
        
        [Test]
        public async Task DeleteObject_should_remove_element()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.CollectionFakeObjectXDocument));
        
            var provider = new PersistentObjectProviderXml<FakeObject, Guid>(_resolver, _documentProvider, _serviceProvider, _options);
        
            await provider.DeleteObjectAsync(FakeObject.SecondInstanceIdentifier);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.SingeFakeObjectXDocument.ToString()));
        }
    }
}
