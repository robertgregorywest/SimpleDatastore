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
        public void FakeObject_should_be_persistent_object()
        {
            Assert.IsTrue(FakeObject.Instance.GetType().IsAPersistentObject());
        }
        
        [Test]
        public void IList_should_be_a_persistent_object_enumerable()
        {
            var list = new List<Widget>();
            var type = list.GetType();
            Assert.IsTrue(type.IsAPersistentObjectEnumerable());
        }

        [Test]
        public void Widget_GetValidProperties_should_not_include_NonPersistedProperty()
        {
            var properties = FakeObject.Instance.GetType().GetValidProperties().ToList();
            Assert.IsFalse(properties.Exists(pi => pi.Name.Equals("NonPersistedProperty")));
        }
    }
}