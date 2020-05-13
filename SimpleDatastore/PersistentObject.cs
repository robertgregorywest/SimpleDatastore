using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly: InternalsVisibleTo("SimpleDatastore.Tests")]

namespace SimpleDatastore
{
    [DataContract]
    public abstract class PersistentObject
    {
        internal const string Identifier = "id";

        [DataMember(Name = Identifier, IsRequired = true)]
        public Guid Id { get; set; } = Guid.Empty;
    }
}
