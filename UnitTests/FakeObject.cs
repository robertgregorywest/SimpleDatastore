using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleDatastore;

namespace UnitTests
{
    public class FakeObject : PersistentObject
    {
        public static Guid FakeObjectIdentifier
        {
            get
            {
                return new Guid("675b689d-db4e-43ed-94dd-591f73a0fc74");
            }
        }

        public static FakeObject Instance
        {
            get
            {
                return new FakeObject() { Id = FakeObjectIdentifier };
            }
        }
    }
}
