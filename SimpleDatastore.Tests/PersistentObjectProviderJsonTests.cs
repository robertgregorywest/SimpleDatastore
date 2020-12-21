using System;
using System.Text.Json;
using Example.Domain;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using SimpleDatastore.Tests.Extensions;
using SimpleDatastore.Tests.Utils;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class PersistentObjectProviderJsonTests
    {
        private IItemResolver<Widget, Guid, JsonElement> _resolver;
        private IDocumentProvider<Widget, Guid, JsonDocument> _documentProvider;
        private IServiceProvider _serviceProvider;
        private IOptions<SimpleDatastoreOptions> _options;

        [SetUp]
        public void Setup()
        {
            _resolver = new ItemResolverJson<Widget, Guid, JsonElement>();
            _documentProvider = Substitute.For<IDocumentProvider<Widget, Guid, JsonDocument>>();
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
            using var doc = JsonDocument.Parse(Widgets.WidgetsArrayJson.GetFixtureContent());
            _documentProvider.GetDocument().Returns(doc);
            
            var provider =
                new PersistentObjectProviderJson<Widget, Guid>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = provider.GetObject(Widgets.SomeWidget.Id);
            
            Assert.AreEqual(Widgets.SomeWidget.Id, actual.Id);
        }
        
        [Test]
        public void GetObject_nonexistent_id_should_return_null()
        {
            using var doc = JsonDocument.Parse(Widgets.WidgetsArrayJson.GetFixtureContent());
            _documentProvider.GetDocument().Returns(doc);
            
            var provider =
                new PersistentObjectProviderJson<Widget, Guid>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = provider.GetObject(Guid.Empty);
            
            Assert.IsNull(actual);
        }
        
        [Test]
        public void GetCollection_not_persistChildren_should_return_collection()
        {
            using var doc = JsonDocument.Parse(Widgets.WidgetsArrayJson.GetFixtureContent());
            _documentProvider.GetDocument().Returns(doc);
            
            var provider =
                new PersistentObjectProviderJson<Widget, Guid>(_resolver, _documentProvider, _serviceProvider, _options);

            var actual = provider.GetCollection();

            actual.Should()
                .NotBeEmpty()
                .And.HaveCount(c => c == 2)
                .And.BeEquivalentTo(Widgets.SomeWidget, Widgets.AnotherWidget);
        }

        [Test]
        public void DeleteObject_should_remove_object()
        {
            using var doc = JsonDocument.Parse(Widgets.WidgetsArrayJson.GetFixtureContent());
            _documentProvider.GetDocument().Returns(doc);

            JsonElement actual = default;

            _documentProvider
                .When(dp => dp.SaveDocument(Arg.Any<JsonDocument>()))
                .Do(y => actual = y.Arg<JsonDocument>().RootElement.Clone());
            
            var provider =
                new PersistentObjectProviderJson<Widget, Guid>(_resolver, _documentProvider, _serviceProvider, _options);

            provider.DeleteObject(Widgets.AnotherWidget.Id);

            var comparer = new JsonElementComparer();
            
            var expected = Widgets.SomeWidgetArrayJson.GetFixtureAsJsonElement();
    
            Assert.IsTrue(comparer.Equals(expected, actual));
        }
    }
}