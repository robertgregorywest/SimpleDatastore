using System.Text.Json;
using Example.Domain;
using SimpleDatastore.Tests.Extensions;

namespace SimpleDatastore.Tests
{
    public static class Employees
    {
        public static Employee Fred => new Employee() {Id = 1, Name = "Fred"};
        
        public const string FredObjectJson = "Example.Domain.Employee.Fred.Object.json";
        
        internal static JsonElement FredJsonElement => FredObjectJson.GetFixtureAsJsonElement();
    }
}