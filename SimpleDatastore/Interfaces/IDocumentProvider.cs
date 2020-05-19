using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SimpleDatastore.Interfaces
{
    public interface IDocumentProvider<T> where T : PersistentObject
    {
        Task<XDocument> GetDocumentAsync();
        Task SaveDocumentAsync(XDocument document);
    }
}
