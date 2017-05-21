using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class FNC : StructuredField
    {
        private static string _abbr = "FNC";
        private static string _title = "Font Control";
        private static string _desc = "Provides defaults and information about the font character set.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(1, Lookups.DataTypes.CODE, "Pattern Technology")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x05, "Laser Matrix N-bit Wide" },
                    { 0x1E, "CID Keyed Font (Type 0)" },
                    { 0x1F, "PFB (Type 1)" }
                }
            },
            new Offset(2, Lookups.DataTypes.EMPTY, ""),
            new Offset(3, Lookups.DataTypes.BITS, "Font Use Flags")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Normal Printing|MICR Printing" },
                    { 0x01, "Complete Font|Extension Font" },
                    { 0x06, "Varied Raster Pattern Size|Fixed Raster Pattern Size" }
                }
            },
            new Offset(4, Lookups.DataTypes.CODE, "X Unit Base")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Fixed Metrics, 10 inches" },
                    { 0x02, "Relative Metrics" }
                }
            },
            new Offset(5, Lookups.DataTypes.CODE, "Y Unit Base")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Fixed Metrics, 10 inches" },
                    { 0x02, "Relative Metrics" }
                }
            },
            new Offset(6, Lookups.DataTypes.UBIN, "X Units Per Inch"),
            new Offset(8, Lookups.DataTypes.UBIN, "Y Units Per Inch"),
            new Offset(10, Lookups.DataTypes.UBIN, "Max Character Box Width"),
            new Offset(12, Lookups.DataTypes.UBIN, "Max Character Box Height"),
            new Offset(14, Lookups.DataTypes.UBIN, "FNO Repeating Group Length"),
            new Offset(15, Lookups.DataTypes.UBIN, "FNI Repeating Group Length"),
            new Offset(16, Lookups.DataTypes.CODE, "Pattern Data Alignment Code")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "1 Byte Alignment" },
                    { 0x02, "4 Byte Alignment" },
                    { 0x03, "8 Byte Alignment" }
                }
            },
            new Offset(17, Lookups.DataTypes.UBIN, "Raster Pattern Data Count"),
            new Offset(20, Lookups.DataTypes.UBIN, "FNP Repeating Group Length"),
            new Offset(21, Lookups.DataTypes.UBIN, "FNM Repeating Group Length"),
            new Offset(22, Lookups.DataTypes.CODE, "Resolution X Unit Base") { Mappings = new Dictionary<byte, string>() { { 0x00, "10 Inches" } } },
            new Offset(23, Lookups.DataTypes.CODE, "Resolution Y Unit Base") { Mappings = new Dictionary<byte, string>() { { 0x00, "10 Inches" } } },
            new Offset(24, Lookups.DataTypes.EMPTY, "Units Per X Base (Need double byte mapping support!)"),
            new Offset(26, Lookups.DataTypes.EMPTY, "Units Per Y Base (Need double byte mapping support!)"),
            new Offset(28, Lookups.DataTypes.UBIN, "Outline Pattern Data Count"),
            new Offset(32, Lookups.DataTypes.EMPTY, ""),
            new Offset(35, Lookups.DataTypes.UBIN, "FNN Repeating Group Length"),
            new Offset(36, Lookups.DataTypes.UBIN, "FNN Data Count"),
            new Offset(40, Lookups.DataTypes.UBIN, "FNN Name Count"),
            new Offset(42, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        protected override List<Offset> Offsets => _oSets;

        public FNC(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }
    }
}