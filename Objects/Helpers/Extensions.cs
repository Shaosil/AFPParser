using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AFPParser
{
    public static class Extensions
    {
        public static Regex RegexReadableText = new Regex(@"[^\w\s\p{P}]");

        public static byte[] GetByteArrayFromHexString(string hex)
        {
            // Return a byte array from a structured field identifier hex string
            List<byte> byteArray = new List<byte>();

            // Convert every two characsters to a byte using HexNumber format provider
            for (int i = 0; i < 6; i += 2)
                if (i < hex.Length + 1)
                    byteArray.Add(byte.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));

            return byteArray.ToArray();
        }
    }
}
