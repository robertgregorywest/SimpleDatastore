using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SimpleDatastore
{
    [DataContract]
    public abstract class PersistentObject : IEquatable<PersistentObject>, IComparable<PersistentObject>
    {
        public const string Identifier = "id";

        private Guid _Id = Guid.Empty;
        [DataMember(Name = Identifier, IsRequired = true)]
        public Guid Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public bool Equals(PersistentObject other)
        {
            if (other == null)
                return false;

            return this.Id.Equals(other.Id);
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as PersistentObject);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }

        public int CompareTo(PersistentObject other)
        {
            return this.Id.CompareTo(other.Id);
        }
    }
}
