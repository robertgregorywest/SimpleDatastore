using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("SimpleDatastore.Tests")]

namespace SimpleDatastore
{
    [DataContract]
    [Serializable]
    public abstract class PersistentObject<TKey> where TKey : struct
    {
        [DataMember(Name = PersistentObject.Identifier, IsRequired = true)]
        [JsonPropertyName(PersistentObject.Identifier)]
        public TKey Id { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public static class PersistentObject
    {
        internal const string Identifier = "id";
        internal const string RootElementName = "data";
        internal const string DataItemName = "dataItem";
        internal const string DataFolder = "App_Data";
    }
}
