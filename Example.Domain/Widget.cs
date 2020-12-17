using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SimpleDatastore;

namespace Example.Domain
{
    public class Widget : PersistentObject<Guid>
    {
        [DataMember(Name = "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [DataMember(Name = "mainPart")]
        [JsonPropertyName("mainPart")]
        public Part MainPart { get; set; }

        [DataMember(Name = "parts")]
        [JsonPropertyName("parts")]
        public IList<Part> Parts { get; set; } = new List<Part>();
        
        [JsonIgnore]
        public Part NonPersistedProperty { get; set; }
    }
}
