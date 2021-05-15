using System;
using System.Linq;
using System.Text.Json;

namespace SimpleDatastore.Extensions
{
    public static class Utf8JsonWriterExtensions
    {
        internal static void WriteUpdate(this Utf8JsonWriter writer, JsonDocument doc, Func<JsonElement, bool> predicate, JsonElement replacement)
        {
            writer.WriteStartArray();
            
            foreach (var element in doc.RootElement.EnumerateArray().Where(predicate))
            {
                element.WriteTo(writer);
            }

            if (replacement.ValueKind != JsonValueKind.Undefined)
            {
                replacement.WriteTo(writer);
            }
            
            writer.WriteEndArray();
        }
    }
}