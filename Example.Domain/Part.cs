using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SimpleDatastore;

namespace Example.Domain
{
    public class Part : PersistentObject<Guid>
    {
        [DataMember(Name = "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
