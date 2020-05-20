using System;

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
    }
}
