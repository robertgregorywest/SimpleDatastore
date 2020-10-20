using NUnit.Framework;
using SimpleDatastore.Tests.Extensions;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class PersistentObjectConverterXmlTests
    {
        [Test]
        public void Write_persistChildren_false_with_child_objects_should_include_objects()
        {
            var result = PersistentObjectConverterXml.Write(Widgets.SomeWidget, null);
            Assert.AreEqual(Widgets.SomeWidgetXml.GetFixtureContent(), result.ToString());
        }
    }
}