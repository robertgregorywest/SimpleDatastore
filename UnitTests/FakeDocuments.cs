using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UnitTests
{
    static class FakeDocuments
    {
        private static string _singleFakeObject = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><id>{0}</id></dataItem></data>", FakeObject.InstanceIdentifier.ToString());
        private static string _emptyDocument = @"<?xml version=""1.0"" encoding=""utf-8""?><data></data>";

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
    }
}
