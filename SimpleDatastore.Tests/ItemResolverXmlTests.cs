using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore.Tests
{
    public class ItemResolverXmlTests
    {
        [Test]
        public async Task GetItemFromNode_Should_Return_Object()
        {
            var repo = Substitute.For<IRepository<FakeObject>>();
            Func<Type, object> activator = Activator.CreateInstance;
            dynamic RepoProvider(Type t) => repo;

            var result = await ItemResolverXml<FakeObject, XElement>.GetObjectFromNodeAsync(
                FakeDocuments.SingleFakeObjectXElement,
                typeof(FakeObject),
                activator,
                RepoProvider,
                true);
        
            Assert.AreEqual(FakeObject.Instance, result);
        }
    }
}
