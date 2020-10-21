using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("SimpleDatastore.Tests")]

namespace SimpleDatastore
{
    [DataContract]
    [Serializable]
    public abstract class PersistentObject
    {
        internal const string Identifier = "id";
        internal const string RootElementName = "data";
        internal const string DataItemName = "dataItem";
        internal const string DataFolder = "App_Data";

        [DataMember(Name = Identifier, IsRequired = true)]
        [JsonPropertyName(Identifier)]
        public Guid Id { get; set; } = Guid.Empty;

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
