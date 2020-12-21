using System;
using System.IO;
using System.Text.Json;

namespace SimpleDatastore.Tests.Extensions
{
    public static class StringExtensions
    {
        public static string GetFixtureContent(this string fixture)
        {
            return File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Fixtures", fixture));
        }

        public static string GetFixturePath(this string fixture)
        {
            return Path.Combine(Environment.CurrentDirectory, "Fixtures", fixture);
        }
        
        internal static JsonElement GetFixtureAsJsonElement(this string fixture)
        {
            using var doc = JsonDocument.Parse(fixture.GetFixtureContent());
            return doc.RootElement.Clone();
        }
    }
}