using System;
using System.Text.Json;
using Example.Domain;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using SimpleDatastore.Tests.Extensions;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class PersistentObjectProviderJsonTests
    {
        private IItemResolver<Widget, JsonElement> _resolver;
        private IDocumentProvider<Widget, JsonDocument> _documentProvider;
        private IServiceProvider _serviceProvider;
        private IOptions<SimpleDatastoreOptions> _options;

        [SetUp]
        public void Setup()
        {
            _resolver = new ItemResolverJson<Widget, JsonElement>();
            _documentProvider = Substitute.For<IDocumentProvider<Widget, JsonDocument>>();
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
        public void GetObject_should_return_object()
        {
            var doc = JsonDocument.Parse(Widgets.WidgetsJson.GetFixtureContent());
            _documentProvider.GetDocument().Returns(doc);
            
            var provider =
                new PersistentObjectProviderJson<Widget>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = provider.GetObject(Widgets.SomeWidget.Id);
            
            Assert.AreEqual(Widgets.SomeWidget.Id, actual.Id);
        }
        
        [Test]
        public void GetObject_nonexistent_id_should_return_null()
        {
            var doc = JsonDocument.Parse(Widgets.WidgetsJson.GetFixtureContent());
            _documentProvider.GetDocument().Returns(doc);
            
            var provider =
                new PersistentObjectProviderJson<Widget>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = provider.GetObject(Guid.Empty);
            
            Assert.IsNull(actual);
        }
        
        [Test]
        public void GetCollection_should_return_collection()
        {
            var doc = JsonDocument.Parse(Widgets.WidgetsJson.GetFixtureContent());
            _documentProvider.GetDocument().Returns(doc);
            
            var provider =
                new PersistentObjectProviderJson<Widget>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = provider.GetCollection();

            actual.Should()
                .NotBeEmpty()
                .And.HaveCount(c => c == 2);
        }
    }
}