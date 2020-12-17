using System;
using System.ComponentModel;
using System.IO;

namespace SimpleDatastore.Extensions
{
    internal static class StringExtensions
    {
        internal static Guid ToGuid(this string s)
        {
            try
            {
                return new Guid(s);
            }
            catch (FormatException)
            {
                return Guid.Empty;
            }
        }
        
        internal static Stream CreateStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        
        internal static T GetKeyFromString<T>(this string source)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromInvariantString(source);
        }
    }
}
