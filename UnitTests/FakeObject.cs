using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleDatastore;

namespace UnitTests
{
    public class FakeObject : PersistentObject
    {
        public const string IDENTIFIER_VALUE = "675b689d-db4e-43ed-94dd-591f73a0fc74";
        public const string ROOT_CACHE_KEY = "UnitTests.FakeObject";
        public const string CACHE_KEY = "UnitTests.FakeObject.675b689d-db4e-43ed-94dd-591f73a0fc74";
        

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
                return new FakeObject() { Id = InstanceIdentifier };
            }
        }
    }
}
