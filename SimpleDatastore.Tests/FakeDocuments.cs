using System.Xml;
using System.Xml.XPath;

namespace SimpleDatastore.Tests
{
    internal static class FakeDocuments
    {
        private static readonly string SingleFakeObjectFixture = $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><name><![CDATA[{FakeObject.NameValue}]]></name><id>{FakeObject.IdentifierValue}</id></dataItem></data>";
        private const string EmptyDocumentFixture = @"<?xml version=""1.0"" encoding=""utf-8""?><data></data>";
        private static readonly string CollectionFixture = $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><name><![CDATA[{FakeObject.NameValue2}]]></name><id>{FakeObject.IdentifierValue2}</id></dataItem><dataItem><name><![CDATA[{FakeObject.NameValue}]]></name><id>{FakeObject.IdentifierValue}</id></dataItem></data>";

        public static XmlDocument SingleFakeObjectDocument
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml(SingleFakeObjectFixture);
                return doc;
            }
        }

        public static XmlDocument EmptyDocument
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml(EmptyDocumentFixture);
                return doc;
            }
        }
        
        public static XmlDocument CollectionDocument
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml(CollectionFixture);
                return doc;
            }
        }

        public static XPathNavigator SingleFakeObjectNavigator
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml(SingleFakeObjectFixture);
                var nav = doc.CreateNavigator();
                nav.MoveToFirstChild();
                var navCurrent = nav.SelectSingleNode($"{Constants.DataItemName}[{PersistentObject.Identifier} = \"{FakeObject.IdentifierValue}\"]");
                return navCurrent;
            }
        }
    }
}
