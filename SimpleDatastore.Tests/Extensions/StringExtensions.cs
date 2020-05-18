using System;
using System.IO;

namespace SimpleDatastore.Tests.Extensions
{
    public static class StringExtensions
    {
        public static string GetFixtureXml(this string fixture)
        {
            return File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Fixtures", fixture));
        }
    }
}