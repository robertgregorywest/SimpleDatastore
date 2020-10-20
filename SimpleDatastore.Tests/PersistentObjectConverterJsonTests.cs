using System;
using Example.Domain;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using SimpleDatastore.Tests.Extensions;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class PersistentObjectConverterJsonTests
    {
        [Test]
        public void Write_persistChildren_false_with_child_objects_should_include_objects()
        {
            var result = PersistentObjectConverterJson.Write(Widgets.SomeWidget, null);
            Assert.AreEqual(Widgets.SomeWidgetJson.GetFixtureContent(), result.RootElement.ToString());
        }
        
        [Test]
        public void Write_persistChildren_true_with_child_objects_should_include_ids()
        {
            var repository = Substitute.For<IRepository<Part>>();
            dynamic RepoProvider(Type t) => repository;
            
            var result = PersistentObjectConverterJson.Write(Widgets.SomeWidget, RepoProvider, true);
            Assert.AreEqual(Widgets.SomeWidgetPersistChildrenJson.GetFixtureContent(), result.RootElement.ToString());
        }
        
        [Test]
        public void Write_persistChildren_true_with_child_objects_should_save()
        {
            var repository = Substitute.For<IRepository<Part>>();
            dynamic RepoProvider(Type t) => repository;
            
            var _ = PersistentObjectConverterJson.Write(Widgets.SomeWidget, RepoProvider, true);

            repository.Received(2);
            repository.Received().Save(Arg.Is<Part>(p => p.Id == Parts.SomeWidgetA.Id));
            repository.Received().Save(Arg.Is<Part>(p => p.Id == Parts.SomeWidgetB.Id));
        }
    }
}