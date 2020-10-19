using System;
using System.Buffers;
using System.Text.Json;

namespace SimpleDatastore.Extensions
{
    internal static class JsonElementExtensions
    {
        internal static bool IsPersistentObjectMatchById(this JsonElement element, Guid id)
        {
            return element.TryGetProperty(PersistentObject.Identifier, out var idElement) 
                   && idElement.TryGetGuid(out var guid) && guid == id;
        }
        
        internal static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                element.WriteTo(writer);
            }

            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
        }

        internal static object ToObject(this JsonElement element, Type returnType, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                element.WriteTo(writer);
            }

            return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, returnType, options);
        }
    }
}