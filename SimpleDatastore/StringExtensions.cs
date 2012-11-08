using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDatastore
{
    public static class StringExtensions
    {
        public static Guid ToGuid(this string s)
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
