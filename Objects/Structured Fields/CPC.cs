using System.Collections.Generic;
using System.Linq;

namespace AFPParser.StructuredFields
{
    public class CPC : StructuredField
    {
        private static string _abbr = "CPC";
        private static string _title = "Code Page Control";
        private static string _desc = "Contains information about the Code Page.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Default Graphic Character GID"),
            new Offset(8, Lookups.DataTypes.BITS, "Default Character Use Flags") { Mappings = CommonMappings.CharacterUseFlags },
            new Offset(9, Lookups.DataTypes.CODE, "CPI Repeating Group Length")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x0A, "Single Byte Code Page" },
                    { 0x0B, "Double Byte Code Page" },
                    { 0xFE, "Single Byte Code Page including Unicode" },
                    { 0xFF, "Double Byte Code Page including Unicode" }
                }
            },
            new Offset(10, Lookups.DataTypes.EMPTY, "Space Character Section Number"),
            new Offset(11, Lookups.DataTypes.EMPTY, "Space Character Code Point"),
            new Offset(12, Lookups.DataTypes.BITS, "Code Page Use Flags")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Sorted in ascending character ID order|Sorted in ascending code point order" },
                    { 0x04, "Variable Spacing Disabled|Variable Spacing Enabled" }
                }
            },
            new Offset(13, Lookups.DataTypes.UBIN, "Unicode scalar value mapped to Default GCGID")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed data
        public bool IsSingleByteCodePage => Data.Length > 9 && new byte[] { 0x0A, 0xFE }.Contains(Data[9]);
        public byte VariableSpaceCharacter => Data[11];

        public CPC(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
    }
}