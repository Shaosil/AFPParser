using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AFPParser
{
    public static class Extensions
    {
        public static Regex RegexReadableText = new Regex("[\\w\\s]*");

        public static List<string> GetNamesOfType(this IEnumerable<AFPFile.Resource> self, AFPFile.Resource.eResourceType rType)
        {
            // Return all non-blank names of resources of the specified type
            return self.Where(r => r.ResourceType == rType && !string.IsNullOrWhiteSpace(r.ResourceName))
                .Select(r => r.ResourceName).ToList();
        }

        public static AFPFile.Resource OfTypeAndName(this IEnumerable<AFPFile.Resource> self, AFPFile.Resource.eResourceType rType, string rName)
        {
            return self.FirstOrDefault(r => r.ResourceType == rType && r.ResourceName == rName);
        }

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
