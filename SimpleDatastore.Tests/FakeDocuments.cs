﻿using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using SimpleDatastore.Tests.Extensions;

namespace SimpleDatastore.Tests
{
    internal static class FakeDocuments
    {
        private static readonly string SingleFakeObjectFixture = $@"<dataItem><name><![CDATA[{FakeObject.NameValue}]]></name><id>{FakeObject.IdentifierValue}</id></dataItem>";
        private static readonly string SingleFakeObjectDocumentFixture = $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><name><![CDATA[{FakeObject.NameValue}]]></name><id>{FakeObject.IdentifierValue}</id></dataItem></data>";
        private static string CollectionDocument(string firstName, string secondName)
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?><data><dataItem><name><![CDATA[{secondName}]]></name><id>{FakeObject.IdentifierValue2}</id></dataItem><dataItem><name><![CDATA[{firstName}]]></name><id>{FakeObject.IdentifierValue}</id></dataItem></data>";
        }
        
        private const string EmptyDocumentXml = "Empty.xml";
        private const string EmptyDocumentJson = "Empty.json";
        private const string SingleFakeObjectArray = "FakeObject.Array.json";

        private static readonly string CollectionDocumentFixture = CollectionDocument(FakeObject.NameValue, FakeObject.NameValue2);
        private static readonly string CollectionDocumentFixtureUpdated = CollectionDocument(FakeObject.NameValue, FakeObject.NameValue2Updated);
        
        internal static XDocument EmptyXDocument => XDocument.Load(EmptyDocumentXml.GetFixturePath());
        internal static XElement SingleFakeObjectXElement => XElement.Parse(SingleFakeObjectFixture);
        internal static XDocument SingeFakeObjectXDocument => XDocument.Parse(SingleFakeObjectDocumentFixture);
        internal static XDocument CollectionFakeObjectXDocument => XDocument.Parse(CollectionDocumentFixture);
        internal static XDocument CollectionFakeObjectXDocumentUpdated => XDocument.Parse(CollectionDocumentFixtureUpdated);
        internal static JsonDocument EmptyJsonDocument => JsonDocument.Parse(EmptyDocumentJson.GetFixtureContent());
        private static JsonDocument SingleFakeObjectJsonDocument => JsonDocument.Parse(SingleFakeObjectArray.GetFixtureContent());

        internal static JsonElement InstanceJsonElement
        {
            get
            {
                using var doc = SingleFakeObjectJsonDocument;
                return doc.RootElement.EnumerateArray().FirstOrDefault().Clone();
            }
        }
    }
}
