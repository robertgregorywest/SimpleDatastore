using System;
using Example.Domain;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using SimpleDatastore.Tests.Extensions;
using SimpleDatastore.Tests.Utils;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class PersistentObjectConverterJsonTests
    {
        [Test]
        public void Write_persistChildren_false_with_child_objects_should_include_objects()
        {
            var actual = PersistentObjectConverterJson.Write(Widgets.SomeWidget, null);
            Assert.AreEqual(Widgets.SomeWidgetObjectJson.GetFixtureContent(), actual.ToString());
        }
        
        [Test]
        public void Write_persistChildren_true_with_child_objects_should_include_ids()
        {
            var repository = Substitute.For<IRepository<Part>>();
            dynamic RepoProvider(Type t) => repository;
            
            var actual = PersistentObjectConverterJson.Write(Widgets.SomeWidget, RepoProvider, true);
            Assert.AreEqual(Widgets.SomeWidgetPersistChildrenObjectJson.GetFixtureContent(), actual.ToString());
        }
        
        [Test]
        public void Write_persistChildren_true_with_child_objects_should_save()
        {
            var repository = Substitute.For<IRepository<Part>>();
            dynamic RepoProvider(Type t) => repository;
            
            var _ = PersistentObjectConverterJson.Write(Widgets.SomeWidget, RepoProvider, true);
            
            repository.Received().Save(Arg.Is<Part>(p => p.Id == Parts.SomeWidgetA.Id));
            repository.Received().Save(Arg.Is<Part>(p => p.Id == Parts.SomeWidgetB.Id));
        }

        [Test]
        public void Write_widget_persistChildren_true_with_child_objects_should_have_child_ids()
        {
            var saveCalled = false;
            var repository = Substitute.For<IRepository<Part>>();
            dynamic RepoProvider(Type t) => repository;
            repository.WhenForAnyArgs(x => x.Save(default)).Do(x => saveCalled = true);
            
            var actual = PersistentObjectConverterJson.Write(Widgets.SomeWidget, RepoProvider, true);
            Assert.IsTrue(saveCalled);
            Assert.AreEqual(Widgets.SomeWidgetPersistChildrenObjectJson.GetFixtureContent(), actual.ToString());
        }
        
        [Test]
        public void Write_fakeObject_persistChildren_true_should_serialize()
        {
            var actual = PersistentObjectConverterJson.Write(FakeObject.Instance, null, true);
            var expected = FakeDocuments.InstanceJsonElement;
            var comparer = new JsonElementComparer();
            Assert.IsTrue(comparer.Equals(expected, actual));
        }
    }
}