using System;
using System.Collections.Generic;
using System.Linq;
using Example.Domain;
using NUnit.Framework;
using SimpleDatastore.Extensions;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void Widget_should_be_PersistentObject()
        {
            Assert.IsTrue(typeof(Widget).IsPersistentObject());
        }
        
        [Test]
        public void NullableInt_should_not_be_PersistentObject()
        {
            Assert.IsFalse(typeof(int?).IsPersistentObject());
        }
        
        [Test]
        public void FakeObject_should_be_persistent_object()
        {
            Assert.IsTrue(FakeObject.Instance.GetType().IsPersistentObject());
        }
        
        [Test]
        public void IList_should_be_a_persistent_object_enumerable()
        {
            var list = new List<Widget>();
            var type = list.GetType();
            Assert.IsTrue(type.IsPersistentObjectEnumerable());
        }

        [Test]
        public void FakeObject_GetValidProperties_should_not_include_NonPersistedProperty()
        {
            var properties = FakeObject.Instance.GetType().PersistedProperties().ToList();
            Assert.IsFalse(properties.Exists(pi => pi.Name.Equals("NonPersistedProperty")));
        }
    }
}