using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using SimpleDatastore;

namespace WebExample.Domain
{
    public class Part : PersistentObject
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}