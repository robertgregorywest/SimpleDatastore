using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDatastore
{
    public static class TypeHelper
    {
        public static bool TypeIsAPersistentObjectList(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(IList<>)) && typeof(PersistentObject).IsAssignableFrom(type.GetGenericArguments()[0]))
            {
                return true;
            }
            return false;
        }
    }
}
