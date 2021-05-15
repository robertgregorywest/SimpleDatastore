using System;
using System.Collections.Generic;
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

        public const string WidgetsArrayJson = "Example.Domain.Widget.Array.json";
        public const string WidgetsPersistChildrenArrayJson = "Example.Domain.Widget.PersistChildren.Array.json";
        public const string SomeWidgetPersistChildrenObjectJson = "Example.Domain.Widget.SomeWidget.PersistChildren.Object.json";
        public const string SomeWidgetXml = "Example.Domain.Widget.SomeWidget.xml";
        public const string SomeWidgetPersistChildrenXml = "Example.Domain.Widget.SomeWidget.PersistChildren.xml";
        public const string SomeWidgetObjectJson = "Example.Domain.Widget.SomeWidget.Object.json";
        public const string SomeWidgetArrayJson = "Example.Domain.Widget.SomeWidget.Array.json";

        internal static JsonElement SomeWidgetJsonElement => SomeWidgetPersistChildrenObjectJson.GetFixtureAsJsonElement();
        
        internal static JsonDocument EmptyDocument => JsonDocument.Parse(JsonSerializer.Serialize(new List<Widget>()));
    }
}