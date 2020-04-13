using System.Xml;
using System.Xml.XPath;

namespace SimpleDatastore.Tests
{
    internal static class FakeDocuments
    {
        private static readonly string SingleFakeObjectFixture = $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><id>{FakeObject.IdentifierValue}</id><name><![CDATA[{FakeObject.NameValue}]]></name></dataItem></data>";
        private const string EmptyDocumentFixture = @"<?xml version=""1.0"" encoding=""utf-8""?><data></data>";
        private static readonly string UnsortedListFixture = $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><id>{FakeObject.IdentifierValue2}</id><name><![CDATA[{FakeObject.NameValue2}]]></name></dataItem><dataItem><id>{FakeObject.IdentifierValue}</id><name><![CDATA[{FakeObject.NameValue}]]></name></dataItem></data>";

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

        public static XmlDocument UnsortedList
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml(UnsortedListFixture);
                return doc;
            }
        }

        public static XPathNavigator SingleFakeObjectNavigtor
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
