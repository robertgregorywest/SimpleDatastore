using System;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Tests.Extensions;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class PersistentObjectJsonSerializerTests
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
        public void Write_persistChildren_false_with_child_objects_should_have_child_objects()
        {
            dynamic RepoProvider(Type t) => _provider;
            var result = PersistentObjectJsonSerializer.Write(Widgets.SomeWidget, RepoProvider, false);
            Assert.AreEqual(Widgets.SomeWidgetJson.GetFixtureContent(), result.RootElement.ToString());
        }
        
        [Test]
        public void Write_persistChildren_true_with_child_objects_should_have_child_ids()
        {
            dynamic RepoProvider(Type t) => _provider;
            var result = PersistentObjectJsonSerializer.Write(Widgets.SomeWidget, RepoProvider, true);
            Assert.AreEqual(Widgets.SomeWidgetPersistChildrenJson.GetFixtureContent(), result.RootElement.ToString());
        }
    }
}