using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFPParser
{
    public static class Lookups
    {
        public enum DataTypes { Bits, Char, Code, Triplets, Ubin };

        public static Dictionary<string, Type> StructuredFields = new Dictionary<string, Type>()
        {
            { "D3EEEE", typeof(NOP) } // No Operation
        };
    }
}
