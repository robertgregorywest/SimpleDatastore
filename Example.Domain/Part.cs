using System.Runtime.Serialization;
using SimpleDatastore;

namespace Example.Domain
{
    public class Part : PersistentObject
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
