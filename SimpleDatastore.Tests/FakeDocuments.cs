using System.Xml;
using System.Xml.XPath;

namespace SimpleDatastore.Tests
{
    internal static class FakeDocuments
    {
        private static readonly string _singleFakeObject = $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><id>{FakeObject.IdentifierValue}</id><name><![CDATA[{FakeObject.NameValue}]]></name></dataItem></data>";
        private static readonly string _emptyDocument = @"<?xml version=""1.0"" encoding=""utf-8""?><data></data>";
        private static readonly string _unsortedList = $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><id>{FakeObject.IdentifierValue2}</id><name><![CDATA[{FakeObject.NameValue2}]]></name></dataItem><dataItem><id>{FakeObject.IdentifierValue}</id><name><![CDATA[{FakeObject.NameValue}]]></name></dataItem></data>";

        public static XmlDocument SingleFakeObjectDocument
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml(_singleFakeObject);
                return doc;
            }
        }

        public static XmlDocument EmptyDocument
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml(_emptyDocument);
                return doc;
            }
        }

        public static XmlDocument UnsortedList
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml(_unsortedList);
                return doc;
            }
        }

        public static XPathNavigator SingleFakeObjectNavigtor
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml(_singleFakeObject);
                var nav = doc.CreateNavigator();
                nav.MoveToFirstChild();
                var navCurrent = nav.SelectSingleNode($"{Constants.DataItemName}[{PersistentObject.Identifier} = \"{FakeObject.IdentifierValue}\"]");
                return navCurrent;
            }
        }
    }
}
