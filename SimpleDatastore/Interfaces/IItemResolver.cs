using System.Threading.Tasks;
using System.Xml.XPath;

namespace SimpleDatastore.Interfaces
{
    public interface IItemResolver<T> where T : PersistentObject
    {
        Task<T> GetItemFromNodeAsync(XPathNavigator nav);
    }
}
