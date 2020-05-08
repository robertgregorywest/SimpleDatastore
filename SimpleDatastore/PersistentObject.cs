using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly: InternalsVisibleTo("SimpleDatastore.Tests")]

namespace SimpleDatastore
{
    [DataContract]
    public abstract class PersistentObject : IEquatable<PersistentObject>, IComparable<PersistentObject>
    {
        internal const string Identifier = "id";

        [DataMember(Name = Identifier, IsRequired = true)]
        public Guid Id { get; set; } = Guid.Empty;

        public bool Equals(PersistentObject other) => other != null && Id.Equals(other.Id);

        public override bool Equals(object other) => Equals(other as PersistentObject);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Id.ToString();

        public int CompareTo(PersistentObject other) => Id.CompareTo(other.Id);
    }
}
