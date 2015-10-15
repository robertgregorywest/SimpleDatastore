using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SimpleDatastore;

namespace Example.Domain
{
    public class Widget : PersistentObject, IComparable<Widget>
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "parts")]
        public IList<Part> Parts { get; set; }

        public int CompareTo(Widget other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}