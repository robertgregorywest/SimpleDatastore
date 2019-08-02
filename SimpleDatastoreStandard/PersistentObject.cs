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
            get { return _id; }
            set { _id = value; }
        }

        public bool Equals(PersistentObject other)
        {
            return other != null && this.Id.Equals(other.Id);
        }

        public override bool Equals(object other)
        {
            return Equals(other as PersistentObject);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public int CompareTo(PersistentObject other)
        {
            return Id.CompareTo(other.Id);
        }
    }
}
