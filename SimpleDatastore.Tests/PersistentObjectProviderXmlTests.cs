﻿using System;
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
        private IItemResolverXml<FakeObject> _resolver;
        private IDocumentProviderXml<FakeObject> _documentProvider;
        private IServiceProvider _serviceProvider;
        private IOptions<SimpleDatastoreOptions> _options;

        [SetUp]
        public void Setup()
        {
            _resolver = Substitute.For<IItemResolverXml<FakeObject>>();
            _documentProvider = Substitute.For<IDocumentProviderXml<FakeObject>>();
            _serviceProvider = Substitute.For<IServiceProvider>();
            _options = Substitute.For<IOptions<SimpleDatastoreOptions>>();
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
        public async Task GetObjectAsync_should_send_element_to_resolver()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.SingeFakeObjectXDocument));
            _resolver.GetItemFromNodeAsync(null).ReturnsForAnyArgs(FakeObject.Instance);
        
            var provider = new PersistentObjectProviderXml<FakeObject>(_resolver, _documentProvider, _serviceProvider, _options);

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
            _resolver = new ItemResolverXml<FakeObject>(serviceProvider, _options);
        
            var provider = new PersistentObjectProviderXml<FakeObject>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = await provider.GetCollectionAsync();
            
            actual.Should()
                .NotBeEmpty()
                .And.HaveCount(c => c == 2)
                .And.ContainInOrder(FakeObject.SecondInstance, FakeObject.Instance);
        }
        
        [Test]
        public async Task GetCollectionAsync_empty_document_should_return_empty_collection()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.EmptyXDocument));
            var serviceProvider = Substitute.For<IServiceProvider>();
            _resolver = new ItemResolverXml<FakeObject>(serviceProvider, _options);
        
            var provider = new PersistentObjectProviderXml<FakeObject>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = await provider.GetCollectionAsync();
            
            actual.Should()
                .NotBeNull()
                .And.BeEmpty();
        }

        [Test]
        public async Task SaveObject_should_save_document_with_empty_document()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.EmptyXDocument));
        
            var provider = new PersistentObjectProviderXml<FakeObject>(_resolver, _documentProvider, _serviceProvider, _options);
        
            await provider.SaveObjectAsync(FakeObject.Instance);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.SingeFakeObjectXDocument.ToString()));
        }
        
        [Test]
        public async Task SaveObject_should_update_existing_object()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.CollectionFakeObjectXDocument));
            _resolver = new ItemResolverXml<FakeObject>(_serviceProvider, _options);
        
            var provider = new PersistentObjectProviderXml<FakeObject>(_resolver, _documentProvider, _serviceProvider, _options);
            
            var actual = FakeObject.SecondInstance;
            actual.Name = FakeObject.NameValue2Updated;
        
            await provider.SaveObjectAsync(actual);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.CollectionFakeObjectXDocumentUpdated.ToString()));
        }
        
        [Test]
        public async Task DeleteObject_should_save_empty_document()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.SingeFakeObjectXDocument));
        
            var provider = new PersistentObjectProviderXml<FakeObject>(_resolver, _documentProvider, _serviceProvider, _options);
        
            await provider.DeleteObjectAsync(FakeObject.InstanceIdentifier);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.EmptyXDocument.ToString()));
        }
        
        [Test]
        public async Task DeleteObject_should_remove_element()
        {
            _documentProvider.GetDocumentAsync().Returns(Task.FromResult(FakeDocuments.CollectionFakeObjectXDocument));
        
            var provider = new PersistentObjectProviderXml<FakeObject>(_resolver, _documentProvider, _serviceProvider, _options);
        
            await provider.DeleteObjectAsync(FakeObject.SecondInstanceIdentifier);
        
            await _documentProvider.Received().SaveDocumentAsync(Arg.Is<XDocument>(d => d.ToString() == FakeDocuments.SingeFakeObjectXDocument.ToString()));
        }
    }
}
