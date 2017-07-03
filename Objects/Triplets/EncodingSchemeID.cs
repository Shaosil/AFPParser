using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class EncodingSchemeID : Triplet
    {
        private static string _desc = "Specifies the encoding scheme associated with a code page.";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public EncodingSchemeID(byte id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            // Encoding scheme (only first byte out of two used)
            bool[] fullEncSchemeArray = GetBitArray(Data[0]);
            sb.AppendLine("Encoding Scheme Flags:");
            Dictionary<int, string> encStructures = new Dictionary<int, string>()
            {
                { 0, "Not Specified" },
                { 2, "IBM-PC Data" },
                { 3, "IBM-PC Display" },
                { 6, "EBCDIC Presentation" },
                { 7, "UTF-16" },
                { 8, "Unicode Presentation" }
            };

            // Normalize bits 0-3 and 4-7
            bool[] encodingStructure = new bool[4] { false, false, false, false }.Concat(fullEncSchemeArray.Take(4)).ToArray();
            bool[] bytesPerCodePoint = new bool[4] { false, false, false, false }.Concat(fullEncSchemeArray.Skip(4).Take(4)).ToArray();

            // Plop each one into an array of ints
            int[] resultInts = new int[2];
            new BitArray(encodingStructure).CopyTo(resultInts, 0);
            new BitArray(bytesPerCodePoint).CopyTo(resultInts, 1);

            // Write results
            if (!encStructures.ContainsKey(resultInts[0])) resultInts[0] = 0;
            sb.AppendLine("* Encoding Structure: " + encStructures[resultInts[0]]);
            if (resultInts[1] == 2) sb.AppendLine("* Fixed Double Byte");
            else sb.AppendLine("* Fixed Single Byte");

            // Encoding scheme for user data
            sb.AppendLine("Encoding Scheme User Data Flags:");
            fullEncSchemeArray = GetBitArray(GetSectionedData(2, 2));

            // Normalize bits 0-3, 4-7, and 8-15
            encodingStructure = new bool[4] { false, false, false, false }.Concat(fullEncSchemeArray.Take(4)).ToArray();
            bytesPerCodePoint = new bool[4] { false, false, false, false }.Concat(fullEncSchemeArray.Skip(4).Take(4)).ToArray();
            bool[] codeExtensionMethod = fullEncSchemeArray.Skip(8).Take(8).ToArray();

            // Plop each one into an array of ints
            resultInts = new int[3];
            new BitArray(encodingStructure).CopyTo(resultInts, 0);
            new BitArray(bytesPerCodePoint).CopyTo(resultInts, 1);
            new BitArray(codeExtensionMethod).CopyTo(resultInts, 2);

            // Write results
            if (resultInts[0] == 7) sb.AppendLine("* Encoding Structure: UTF-16");
            else sb.AppendLine("* Encoding Structure: Unknown");
            if (resultInts[1] == 8) sb.AppendLine("* UTF-n Variable Number of Bytes");
            else sb.AppendLine("* Fixed Double Byte");
            if (resultInts[2] == 7) sb.AppendLine("* Code Extension Method: UTF-8 Universal Transformation");
            else sb.AppendLine("* Code Extension Method: None specified");

            return sb.ToString();
        }
    }
}