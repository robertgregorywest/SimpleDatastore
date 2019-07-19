using System.Xml.XPath;

namespace SimpleDatastore.Interfaces
{
    public interface IXmlResolver<T> where T : PersistentObject
    {
        T GetItemFromNode(XPathNavigator nav);
    }
}
