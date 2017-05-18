using System.Text;
using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class PresentationSpaceMixingRule : Triplet
    {
        private static string _desc = "Specifies the rules for establishing the color attribute of areas formed by the intersection of two presentation spaces.";
        private static List<Offset> _oSets = new List<Offset>();

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

        public PresentationSpaceMixingRule(byte[] allData) : base(allData) { }

        public override string GetDescription()
        {
            // Offset is NULL since we are handling the whole data section (only one offset defined)

            StringBuilder sb = new StringBuilder();

            // Predefine our values for the mixing spaces
            Dictionary<byte, string> mixingSpaces = new Dictionary<byte, string>()
            {
                { 0x70, "Background on Background" },
                { 0x71, "Background on Foreground" },
                { 0x72, "Foreground on Background" },
                { 0x73, "Foreground on Foreground" },
            };

            // Predefine our values for the mixing descriptions
            Dictionary<byte, string> values = new Dictionary<byte, string>()
            {
                { 0x01, "Overpaint" },
                { 0x02, "Underpaint" },
                { 0x03, "Blend" },
                { 0xFF, "MO:DCA Default Mixing Rule" }
            };

            // Position 2 has up to four two-byte keywords
            for (int i = 2; i <= Data.Length - 2; i += 2)
            {
                byte keyword = Data[i];
                byte value = Data[i + 1];

                string mixingSpace = mixingSpaces.ContainsKey(keyword) ? mixingSpaces[keyword] : "(INVALID MIXING SPACE)";
                string mixingValue = values.ContainsKey(value) ? values[value] : "(INVALID MIXING VALUE)";
                sb.AppendLine($"{mixingSpace}: {mixingValue}");
            }

            return sb.ToString();
        }
    }
}