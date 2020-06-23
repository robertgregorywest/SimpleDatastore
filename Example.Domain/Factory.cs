using System.Collections.Generic;
using System.Runtime.Serialization;
using SimpleDatastore;

namespace Example.Domain
{
    public class Factory : PersistentObject
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "widgets")]
        public IList<Widget> Widgets { get; set; }
    }
}