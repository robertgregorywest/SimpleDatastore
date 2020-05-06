using System;
using System.Runtime.Serialization;

namespace SimpleDatastore
{
    [DataContract]
    public abstract class PersistentObject : IEquatable<PersistentObject>, IComparable<PersistentObject>
    {
        internal const string Identifier = "id";

        private Guid _id = Guid.Empty;

        [DataMember(Name = Identifier, IsRequired = true)]
        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        public bool Equals(PersistentObject other) => other != null && Id.Equals(other.Id);

        public override bool Equals(object other) => Equals(other as PersistentObject);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Id.ToString();

        public int CompareTo(PersistentObject other) => Id.CompareTo(other.Id);
    }
}
