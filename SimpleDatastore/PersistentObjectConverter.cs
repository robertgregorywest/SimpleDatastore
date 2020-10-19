using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SimpleDatastore.Extensions;

namespace SimpleDatastore
{
    public class PersistentObjectConverter : JsonConverter<PersistentObject>
    {
        public override PersistentObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, PersistentObject value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Id.ToString());
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsPersistentObject();
        }
    }
}