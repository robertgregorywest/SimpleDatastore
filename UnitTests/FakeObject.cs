using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using SimpleDatastore;

namespace UnitTests
{
    public class FakeObject : PersistentObject, IComparable<FakeObject>
    {
        public const string IDENTIFIER_VALUE = "675b689d-db4e-43ed-94dd-591f73a0fc74";
        public const string NAME_VALUE = "FakeObject name";
        public const string ROOT_CACHE_KEY = "UnitTests.FakeObject";
        public const string CACHE_KEY = "UnitTests.FakeObject.675b689d-db4e-43ed-94dd-591f73a0fc74";
        public const string IDENTIFIER_VALUE_2 = "ab08bec7-835f-49ca-a285-6ba195576305";
        public const string NAME_VALUE_2 = "Second FakeObject name";

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public int CompareTo(FakeObject other)
        {
            return this.Name.CompareTo(other.Name);
        }

        // Utility static properties for testing
        public static Guid InstanceIdentifier
        {
            get
            {
                return new Guid(IDENTIFIER_VALUE);
            }
        }

        public static FakeObject Instance
        {
            get
            {
                return new FakeObject() { Id = InstanceIdentifier, Name = NAME_VALUE };
            }
        }

        public static Guid SecondInstanceIdentifier
        {
            get
            {
                return new Guid(IDENTIFIER_VALUE_2);
            }
        }

        public static FakeObject SecondInstance
        {
            get
            {
                return new FakeObject() { Id = SecondInstanceIdentifier, Name = NAME_VALUE_2 };
            }
        }

        public static List<FakeObject> UnsortedList
        {
            get
            {
                return new List<FakeObject>() { SecondInstance, Instance };
            }
        }

        public static List<FakeObject> SortedList
        {
            get
            {
                return new List<FakeObject>() { Instance, SecondInstance };
            }
        }
    }
}
