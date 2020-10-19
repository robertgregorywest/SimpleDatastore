using System;
using System.Text.Json;

namespace SimpleDatastore.Extensions
{
    internal static class JsonDocumentExtensions
    {
        internal static T ToObject<T>(this JsonDocument document, JsonSerializerOptions options = null)
        {
            return document == null
                ? throw new ArgumentNullException(nameof(document))
                : document.RootElement.ToObject<T>(options);
        }

        internal static object ToObject(this JsonDocument document, Type returnType, JsonSerializerOptions options = null)
        {
            return document == null
                ? throw new ArgumentNullException(nameof(document))
                : document.RootElement.ToObject(returnType, options);
        } 
    }
}