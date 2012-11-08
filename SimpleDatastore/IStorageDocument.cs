using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SimpleDatastore
{
    public interface IStorageDocument<T> where T : PersistentObject
    {
        XmlDocument Get();
        void Save(XmlDocument document);
    }
}
