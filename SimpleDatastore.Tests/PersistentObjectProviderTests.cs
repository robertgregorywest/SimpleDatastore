using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using System.Xml.Linq;
using FluentAssertions;

namespace SimpleDatastore.Tests
{
    public class PersistentObjectProviderTests
    {
        private IItemResolver<FakeObject> _resolver;
        private IDocumentProvider<FakeObject> _documentProvider;

        [SetUp]
        public void Setup()
        {
            _resolver = Substitute.For<IItemResolver<FakeObject>>();
            _documentProvider = Substitute.For<IDocumentProvider<FakeObject>>();
        }

        [TearDown]
        public void Cleanup()
        {
            _resolver = null;
            _documentProvider = null;
        }

        [Test]
        public async Task GetObjectAsync_should_send_element_to_resolver()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.SingeFakeObjectXDocument));
            _resolver.GetItemFromNodeAsync(null).ReturnsForAnyArgs(FakeObject.Instance);
        
            var provider = new PersistentObjectProvider<FakeObject>(_resolver, _documentProvider);

            var actual = await provider.GetObjectAsync(FakeObject.InstanceIdentifier);
            
            Assert.IsNotNull(actual);

            await _resolver.Received()
                .GetItemFromNodeAsync(Arg.Is<XElement>(e => e.ToString() == FakeDocuments.SingleFakeObjectXElement.ToString()));
        }
        
        [Test]
        public async Task GetCollectionAsync_should_return_collection()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.CollectionFakeObjectXDocument));
            var serviceProvider = Substitute.For<IServiceProvider>();
            _resolver = new ItemResolver<FakeObject>(serviceProvider);
        
            var provider = new PersistentObjectProvider<FakeObject>(_resolver, _documentProvider);

            var actual = await provider.GetCollectionAsync();
            
            actual.Should()
                .NotBeEmpty()
                .And.HaveCount(c => c == 2)
                .And.ContainInOrder(FakeObject.SecondInstance, FakeObject.Instance);
        }

        [Test]
        public async Task SaveObject_should_save_document_with_empty_document()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.EmptyXDocument));
        
            var provider = new PersistentObjectProvider<FakeObject>(_resolver, _documentProvider);
        
            await provider.SaveObjectAsync(FakeObject.Instance);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.SingeFakeObjectXDocument.ToString()));
        }
        
        [Test]
        public async Task SaveObject_should_update_existing_object()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.CollectionFakeObjectXDocument));
            var serviceProvider = Substitute.For<IServiceProvider>();
            _resolver = new ItemResolver<FakeObject>(serviceProvider);
        
            var provider = new PersistentObjectProvider<FakeObject>(_resolver, _documentProvider);
            
            var actual = FakeObject.SecondInstance;
            actual.Name = FakeObject.NameValue2Updated;
        
            await provider.SaveObjectAsync(actual);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.CollectionFakeObjectXDocumentUpdated.ToString()));
        }
        
        [Test]
        public async Task DeleteObject_should_save_empty_document()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.SingeFakeObjectXDocument));
        
            var provider = new PersistentObjectProvider<FakeObject>(_resolver, _documentProvider);
        
            await provider.DeleteObjectAsync(FakeObject.InstanceIdentifier);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.EmptyXDocument.ToString()));
        }

        [Test]
        public void BuildXml_with_child_objects_should_persist_ids()
        {
            var result = PersistentObjectProvider<FakeObject>.BuildXml(FakeObject.Instance);
            Assert.AreEqual(result.ToString(), FakeDocuments.SingleFakeObjectXElement.ToString());
        }

        [Test]
        public void GetElementById_should_return_Element()
        {
            var actual = PersistentObjectProvider<FakeObject>.GetElementById(FakeDocuments.SingeFakeObjectXDocument,
                FakeObject.InstanceIdentifier);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(FakeDocuments.SingleFakeObjectXElement.ToString(), actual.ToString());
        }
}
}
