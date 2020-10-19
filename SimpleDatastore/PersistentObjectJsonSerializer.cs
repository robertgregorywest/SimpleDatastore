using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    internal static class PersistentObjectJsonSerializer
    {
        internal static JsonDocument Write<T>(T instance, Func<Type, dynamic> repoProvider, bool persistChildren = false)
            where T : PersistentObject
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            
            foreach (var property in instance.GetType().PersistedProperties())
            {
                 var attributeName = property.GetPropertyName();
                 var value = property.GetValue(instance, null);
                
                if (attributeName == PersistentObject.Identifier)
                {
                    writer.WriteString(attributeName, value.ToString());
                    continue;
                }
                
                if (property.PropertyType.IsPersistentObject() && value is PersistentObject persistentObject)
                {
                    if (persistChildren)
                    {
                        var repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
                        var repository = repoProvider(repositoryType);
                        repository.Save((dynamic) persistentObject);
                        writer.WriteString(attributeName, persistentObject.Id.ToString());
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, value, property.PropertyType, serializeOptions);
                    }
                    continue;
                }
                
                if (property.PropertyType.IsPersistentObjectEnumerable() &&
                    value is IEnumerable<PersistentObject> persistentObjectEnumerable)
                {
                    var list = persistentObjectEnumerable.ToList();
                
                    if (persistChildren)
                    {
                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
                        var repository = repoProvider(repositoryType);
                
                        foreach (var item in list)
                        {
                            repository.Save((dynamic) item);
                        }
                        
                        var flattenedEnumerable = string.Join(",", list);
                        writer.WriteString(attributeName, flattenedEnumerable);
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, value, property.PropertyType, serializeOptions);
                    }
                    continue;
                }
                
                JsonSerializer.Serialize(writer, value, property.PropertyType, serializeOptions);
            }

            stream.Position = 0;
            
            return JsonDocument.Parse(stream);;
        }
    }
}