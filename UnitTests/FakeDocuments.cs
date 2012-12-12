using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UnitTests
{
    static class FakeDocuments
    {
        private static string _singleFakeObject = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><id>{0}</id><name><![CDATA[{1}]]></name></dataItem></data>", FakeObject.IDENTIFIER_VALUE, FakeObject.NAME_VALUE);
        private static string _emptyDocument = @"<?xml version=""1.0"" encoding=""utf-8""?><data></data>";
        private static string _unsortedList = 
            string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><id>{0}</id><name><![CDATA[{1}]]></name></dataItem><dataItem><id>{2}</id><name><![CDATA[{3}]]></name></dataItem></data>",
            FakeObject.IDENTIFIER_VALUE_2, FakeObject.NAME_VALUE_2, FakeObject.IDENTIFIER_VALUE, FakeObject.NAME_VALUE);

        public static XmlDocument SingleFakeObjectDocument
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_singleFakeObject);
                return doc;
            }
        }

        public static XmlDocument EmptyDocument
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_emptyDocument);
                return doc;
            }
        }

        public static XmlDocument UnsortedList
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_unsortedList);
                return doc;
            }
        }
    }
}
