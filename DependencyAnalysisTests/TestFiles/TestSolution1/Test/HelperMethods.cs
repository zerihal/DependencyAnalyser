using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public static class HelperMethods
    {
        public static Guid GetGuidFromString(string guidString)
        {
            Guid guid;
            if (Guid.TryParse(guidString, out guid))
            {
                return guid;
            }
            else
            {
                throw new FormatException($"The string '{guidString}' is not a valid GUID.");
            }
        }
    }
}
