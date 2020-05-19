using System.Xml.Linq;
using SimpleDatastore.Tests.Extensions;

namespace SimpleDatastore.Tests
{
    internal static class FakeDocuments
    {
        private static readonly string SingleFakeObjectFixture = $@"<dataItem><name><![CDATA[{FakeObject.NameValue}]]></name><id>{FakeObject.IdentifierValue}</id></dataItem>";
        private static readonly string SingleFakeObjectDocumentFixture = $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><name><![CDATA[{FakeObject.NameValue}]]></name><id>{FakeObject.IdentifierValue}</id></dataItem></data>";
        private static readonly string CollectionDocumentFixture = $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><name><![CDATA[{FakeObject.NameValue2}]]></name><id>{FakeObject.IdentifierValue2}</id></dataItem><dataItem><name><![CDATA[{FakeObject.NameValue}]]></name><id>{FakeObject.IdentifierValue}</id></dataItem></data>";
        
        private const string EmptyDocumentFilename = "Empty.xml";

        public static XDocument EmptyXDocument => XDocument.Load(EmptyDocumentFilename.GetFixturePath());
        public static XElement SingleFakeObjectXElement => XElement.Parse(SingleFakeObjectFixture);
        public static XDocument SingeFakeObjectXDocument => XDocument.Parse(SingleFakeObjectDocumentFixture);
        public static XDocument CollectionFakeObjectXDocument => XDocument.Parse(CollectionDocumentFixture);
    }
}
