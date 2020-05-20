using NUnit.Framework;
using SimpleDatastore.Extensions;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class XDocumentExtensionsTests
    {
        [Test]
        public void GetElementById_should_return_Element()
        {
            var actual = FakeDocuments.SingeFakeObjectXDocument.GetElementById(FakeObject.InstanceIdentifier);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(FakeDocuments.SingleFakeObjectXElement.ToString(), actual.ToString());
        }
    }
}