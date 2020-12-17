using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SimpleDatastore;

namespace Example.Domain
{
    public class Factory : PersistentObject<Guid>
    {
        [DataMember(Name = "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [DataMember(Name = "widgets")]
        [JsonPropertyName("widgets")]
        public IList<Widget> Widgets { get; set; } = new List<Widget>();
    }
}