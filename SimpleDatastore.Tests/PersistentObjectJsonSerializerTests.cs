using System;
using Example.Domain;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Interfaces;
using SimpleDatastore.Tests.Extensions;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class PersistentObjectJsonSerializerTests
    {
        [Test]
        public void Write_persistChildren_false_with_child_objects_should_have_child_objects()
        {
            var result = PersistentObjectConverterJson.Write(Widgets.SomeWidget, null, false);
            Assert.AreEqual(Widgets.SomeWidgetJson.GetFixtureContent(), result.RootElement.ToString());
        }
        
        [Test]
        public void Write_widget_persistChildren_true_with_child_objects_should_have_child_ids()
        {
            var saveCalled = false;
            var repository = Substitute.For<IRepository<Part>>();
            dynamic RepoProvider(Type t) => repository;
            repository.WhenForAnyArgs(x => x.Save(default)).Do(x => saveCalled = true);
            
            var result = PersistentObjectConverterJson.Write(Widgets.SomeWidget, RepoProvider, true);
            Assert.IsTrue(saveCalled);
            Assert.AreEqual(Widgets.SomeWidgetPersistChildrenJson.GetFixtureContent(), result.RootElement.ToString());
        }
        
        [Test]
        public void Write_fakeObject_persistChildren_true_should_serialize()
        {
            var result = PersistentObjectConverterJson.Write(FakeObject.Instance, null, true);
            Assert.AreEqual(FakeDocuments.InstanceJson.GetFixtureContent(), result.RootElement.ToString());
        }
    }
}