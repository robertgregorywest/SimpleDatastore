﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UnitTests
{
    static class TestDocuments
    {
        private static string _singleTestObject =
string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<data>
  <dataItem>
    <id>{0}</id>
  </dataItem>
</data>", TestObjectIdentifier.ToString());

        public static Guid TestObjectIdentifier
        {
            get
            {
                return new Guid("675b689d-db4e-43ed-94dd-591f73a0fc74");
            }
        }

        public static XmlDocument SingleTestObject
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_singleTestObject);
                return doc;
            }
        }
    }
}
