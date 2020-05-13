using System.Xml.XPath;

namespace SimpleDatastore.Interfaces
{
    public interface IItemResolver<out T> where T : PersistentObject
    {
        T GetItemFromNode(XPathNavigator nav);
    }
}
