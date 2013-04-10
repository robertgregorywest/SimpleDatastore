using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using SimpleDatastore;

namespace WebExample.Domain
{
    public class Widget : PersistentObject, IComparable<Widget>
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        public int CompareTo(Widget other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}