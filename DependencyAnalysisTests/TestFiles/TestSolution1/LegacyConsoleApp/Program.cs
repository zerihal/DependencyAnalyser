using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test;

namespace LegacyConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var stringGuid = "12345678-1234-1234-1234-1234567890ab";
            var guid = HelperMethods.GetGuidFromString(stringGuid);
        }
    }
}
