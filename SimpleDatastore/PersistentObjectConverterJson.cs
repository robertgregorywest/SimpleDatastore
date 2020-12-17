using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using static SimpleDatastore.SimpleDatastoreOptions;

namespace SimpleDatastore
{
    internal static class PersistentObjectConverterJson
    {
        internal static JsonElement Write<T, TKey>(T instance, Func<Type, dynamic> repoProvider, bool persistChildren = false)
            where T : PersistentObject<TKey> 
            where TKey : struct
        {
            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            if (!persistChildren)
            {
                using var doc = JsonDocument.Parse(JsonSerializer.Serialize(instance, serializerOptions));
                return doc.RootElement.Clone();
            }

            var writerOptions = new JsonWriterOptions
            {
                Indented = true
            };

            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, writerOptions);

            writer.WriteStartObject();
            
            foreach (var property in instance.GetType().PersistedProperties(StorageModeOptions.Json))
            { 
                var attributeName = property.GetPropertyName(StorageModeOptions.Json);
                var value = property.GetValue(instance, null);
                 
                if (property.PropertyType.IsPersistentObject())
                {
                    var repositoryType = typeof(IWriteRepository<,>)
                        .MakeGenericType(property.PropertyType, property.PropertyType.GetKeyType());
                    var repository = repoProvider(repositoryType);
                    repository.Save((dynamic) value);
                    writer.WriteString(attributeName, value.ToString());
                    continue;
                }
                
                if (property.PropertyType.IsPersistentObjectEnumerable() && value is IEnumerable<object> persistentObjectEnumerable)
                {
                    var list = persistentObjectEnumerable.ToList();

                    var elementType = property.PropertyType.GetGenericArguments()[0];
                    var repositoryType = typeof(IWriteRepository<,>).MakeGenericType(elementType, elementType.GetKeyType());
                    var repository = repoProvider(repositoryType);

                    foreach (var item in list)
                    {
                        repository.Save((dynamic) item);
                    }

                    writer.WritePropertyName(attributeName);
                    JsonSerializer.Serialize(writer, list.Select(p => p.ToString()), serializerOptions);
                    
                    continue;
                }
                
                writer.WritePropertyName(attributeName);
                JsonSerializer.Serialize(writer, value, property.PropertyType, serializerOptions);
            }
            
            writer.WriteEndObject();
            
            writer.Flush();
            stream.Position = 0;

            using (var doc = JsonDocument.Parse(stream))
            {
                return doc.RootElement.Clone();
            }
        }
    }
}