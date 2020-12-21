using NUnit.Framework;
using SimpleDatastore.Extensions;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class JsonElementExtensionsTests
    {
        [Test]
        public void Employee_should_be_persistent_object_match()
        {
            var result = Employees.FredJsonElement.IsPersistentObjectMatchById(1);
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Incorrect_type_should_not_be_persistent_object_match()
        {
            var result = Widgets.SomeWidgetJsonElement.IsPersistentObjectMatchById(1);
            Assert.IsFalse(result);
        }
    }
}