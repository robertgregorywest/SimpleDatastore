using System;
using System.Text.Json;
using Example.Domain;
using SimpleDatastore.Tests.Extensions;

namespace SimpleDatastore.Tests
{
    public static class Widgets
    {
        public static Widget SomeWidget => new Widget()
        {
            Id = new Guid("6ea4ad00-08ba-4ac1-8e52-54a890eca0e0"),
            Name = "Some Widget", 
            MainPart = Parts.SomeWidgetA,
            Parts = new []{ Parts.SomeWidgetA, Parts.SomeWidgetB }
        };

        public static Widget AnotherWidget => new Widget()
        {
            Id = new Guid("f0d91008-a34c-48ce-acf6-8f89ff106607"),
            Name = "Another Widget",
            MainPart = Parts.AnotherWidgetA,
            Parts = new []{ Parts.AnotherWidgetA }
        };

        public const string WidgetsJson = "Example.Domain.Widget.json";
        public const string WidgetsPersistChildrenJson = "Example.Domain.Widget.PersistChildren.json";
        public const string SomeWidgetPersistChildrenJson = "Example.Domain.Widget.SomeWidget.PersistChildren.json";
        public const string SomeWidgetXml = "Example.Domain.Widget.SomeWidget.xml";
        public const string SomeWidgetPersistChildrenXml = "Example.Domain.Widget.SomeWidget.PersistChildren.xml";
        public const string SomeWidgetJson = "Example.Domain.Widget.SomeWidget.json";
        
        internal static JsonElement SomeWidgetJsonElement
        {
            get
            {
                using var doc = JsonDocument.Parse(SomeWidgetPersistChildrenJson.GetFixtureContent());
                return doc.RootElement.Clone();
            }
        }
    }
}