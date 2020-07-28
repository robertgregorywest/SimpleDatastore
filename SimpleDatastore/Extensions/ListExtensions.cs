using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleDatastore.Extensions
{
    internal static class ListExtensions
    {
        internal static void AddOrReplace<T>(this List<T> list, T item) where T : PersistentObject
        {
            var existingIndex = list.FindIndex(o => o.Id == item.Id);
            
            if (existingIndex > -1)
            {
                list[existingIndex] = item;
            }
            else
            {
                list.Add(item);
            }
        }
    }
}