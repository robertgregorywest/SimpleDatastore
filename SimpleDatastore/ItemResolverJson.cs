using System;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class ItemResolverJson<T, TElement> : IItemResolver<T, string> 
        where T : PersistentObject
    {
        public Task<T> GetItemFromNodeAsync(string element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren)
        {
            throw new NotImplementedException();
        }

        public T GetItemFromNode(string element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren)
        {
            throw new NotImplementedException();
        }
    }
}