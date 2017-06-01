using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class CPD : StructuredField
    {
        private static string _abbr = "CPD";
        private static string _title = "Code Page Descriptor";
        private static string _desc = "Describes the code page.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Code Page Description"),
            new Offset(32, Lookups.DataTypes.UBIN, "Graphic Character GID Length"),
            new Offset(34, Lookups.DataTypes.UBIN, "Number of Coded Graphic Characters Assigned"),
            new Offset(38, Lookups.DataTypes.UBIN, "Graphic Character Set GID"),
            new Offset(40, Lookups.DataTypes.UBIN, "Code Page GID"),
            new Offset(42, Lookups.DataTypes.UBIN, "Encoding Scheme")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "No encoding scheme specified" },
                    { 0x01, "Single byte, encoding not specified" },
                    { 0x02, "Double byte, encoding not specified" },
                    { 0x21, "Single byte, IBM-PC Data" },
                    { 0x61, "Single byte, EBCDIC Presentation" },
                    { 0x62, "Double byte, EBCDIC Presentation" },
                    { 0x82, "Double byte, UCS Presentation" }
                }
            }
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        protected override List<Offset> Offsets => _oSets;

        public CPD(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }
    }
}