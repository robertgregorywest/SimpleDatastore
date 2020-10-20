using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    internal static class PersistentObjectConverterJson
    {
        internal static JsonElement Write<T>(T instance, Func<Type, dynamic> repoProvider, bool persistChildren = false)
            where T : PersistentObject
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
            
            foreach (var property in instance.GetType().PersistedProperties())
            { 
                var attributeName = property.GetPropertyName();
                var value = property.GetValue(instance, null);
                 
                if (property.PropertyType.IsPersistentObject() && value is PersistentObject persistentObject)
                {
                    var repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
                    var repository = repoProvider(repositoryType);
                    repository.Save((dynamic) persistentObject);
                    writer.WriteString(attributeName, persistentObject.Id.ToString());
                    continue;
                }
                
                if (property.PropertyType.IsPersistentObjectEnumerable() &&
                    value is IEnumerable<PersistentObject> persistentObjectEnumerable)
                {
                    var list = persistentObjectEnumerable.ToList();

                    var elementType = property.PropertyType.GetGenericArguments()[0];
                    var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
                    var repository = repoProvider(repositoryType);

                    foreach (var item in list)
                    {
                        repository.Save((dynamic) item);
                    }

                    writer.WritePropertyName(attributeName);
                    JsonSerializer.Serialize(writer, list.Select(p => p.Id), serializerOptions);
                    
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