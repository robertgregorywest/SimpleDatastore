using System;

namespace SimpleDatastore.Extensions
{
    public static class PersistentObjectExtensions
    {
        public static PersistentObject EnsureValidGuid(this PersistentObject persistentObject)
        {
            if (persistentObject.Id == Guid.Empty)
            {
                persistentObject.Id = Guid.NewGuid();
            }

            return persistentObject;
        }
    }
}