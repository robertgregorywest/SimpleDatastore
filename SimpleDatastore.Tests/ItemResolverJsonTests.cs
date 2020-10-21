using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Example.Domain;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class ItemResolverJsonTests
    {
        [Test]
        public async Task GetObjectFromNodeAsync_Should_Return_Object()
        {
            var repo = Substitute.For<IRepository<FakeObject>>();
            Func<Type, object> activator = Activator.CreateInstance;
            dynamic RepoProvider(Type t) => repo;

            var result = await ItemResolverJson<FakeObject, JsonElement>.GetObjectFromNodeAsync(
                FakeDocuments.InstanceJsonElement,
                typeof(FakeObject),
                activator,
                RepoProvider,
                true);
        
            result.Should().Be(FakeObject.Instance);
        }

        [Test]
        public async Task GetObjectFromNodeAsync_persistChildren_true_should_load_children()
        {
            var repo = Substitute.For<IRepository<Part>>();
            repo.LoadAsync(Parts.SomeWidgetA.Id).Returns(Parts.SomeWidgetA);
            repo.LoadCollectionByIdsAsync(Enumerable.Empty<string>()).ReturnsForAnyArgs(Widgets.SomeWidget.Parts);
            
            Func<Type, object> activator = Activator.CreateInstance;
            dynamic RepoProvider(Type t) => repo;

            var result = await ItemResolverJson<Widget, JsonElement>.GetObjectFromNodeAsync(
                Widgets.SomeWidgetJsonElement,
                typeof(Widget),
                activator,
                RepoProvider,
                true) as Widget;

            result.Should().NotBeNull();
            if (result != null)
            {
                result.Id.Should().Be(Widgets.SomeWidget.Id);
                result.MainPart.Id.Should().Be(Widgets.SomeWidget.MainPart.Id);
                result.Parts.Should().HaveCount(2);
            }
        }
    }
}