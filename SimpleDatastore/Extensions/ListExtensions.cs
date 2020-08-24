using System.Collections.Generic;

namespace SimpleDatastore.Extensions
{
    internal static class ListExtensions
    {
        internal static List<T> AddOrReplace<T>(this List<T> list, T item) where T : PersistentObject
        {
            var index = list.FindIndex(t => t.Id.Equals(item.Id));
            
            if (index > -1)
            {
                list[index] = item;
            }
            else
            {
                list.Add(item);
            }

            return list;
        }
    }
}