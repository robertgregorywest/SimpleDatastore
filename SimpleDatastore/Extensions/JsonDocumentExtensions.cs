using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleDatastore.Extensions
{
    internal static class JsonDocumentExtensions
    {
        internal static T Deserialize<T>(this JsonDocument document, JsonSerializerOptions options = null)
        {
            return document == null
                ? throw new ArgumentNullException(nameof(document))
                : document.RootElement.Deserialize<T>(options);
        }

        internal static object Deserialize(this JsonDocument document, Type returnType, JsonSerializerOptions options = null)
        {
            return document == null
                ? throw new ArgumentNullException(nameof(document))
                : document.RootElement.Deserialize(returnType, options);
        } 
    }
}