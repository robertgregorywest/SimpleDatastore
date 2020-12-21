using System;
using System.Buffers;
using System.Text.Json;

namespace SimpleDatastore.Extensions
{
    internal static class JsonElementExtensions
    {
        internal static bool IsPersistentObjectMatchById<TKey>(this JsonElement element, TKey id) where TKey : struct
        {
            if (!element.TryGetProperty(PersistentObject.Identifier, out var idElement))
            {
                return false;
            }

            try
            {
                return Deserialize(idElement, typeof(TKey)).Equals(id);
            }
            catch
            {
                return false;
            }
        }
        
        internal static T Deserialize<T>(this JsonElement element, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                element.WriteTo(writer);
            }

            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
        }

        internal static object Deserialize(this JsonElement element, Type returnType, JsonSerializerOptions options = null)
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