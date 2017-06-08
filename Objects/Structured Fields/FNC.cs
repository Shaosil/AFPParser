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
                    { 0x05, ePatternTech.LaserMatrixNBitWide.ToString() },
                    { 0x1E, ePatternTech.CIDKeyedFont.ToString() },
                    { 0x1F, ePatternTech.PFBType1.ToString() }
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
            new Offset(22, Lookups.DataTypes.CODE, "Resolution X Unit Base") { Mappings = Lookups.CommonMappings.AxisBase },
            new Offset(23, Lookups.DataTypes.CODE, "Resolution Y Unit Base") { Mappings = Lookups.CommonMappings.AxisBase },
            new Offset(24, Lookups.DataTypes.UBIN, "Units Per X Base"),
            new Offset(26, Lookups.DataTypes.UBIN, "Units Per Y Base"),
            new Offset(28, Lookups.DataTypes.UBIN, "Outline Pattern Data Count"),
            new Offset(32, Lookups.DataTypes.EMPTY, ""),
            new Offset(35, Lookups.DataTypes.UBIN, "FNN Repeating Group Length"),
            new Offset(36, Lookups.DataTypes.UBIN, "FNN Data Count"),
            new Offset(40, Lookups.DataTypes.UBIN, "FNN Name Count"),
            new Offset(42, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Custom data
        public enum ePatternTech { LaserMatrixNBitWide, CIDKeyedFont, PFBType1 }
        private Dictionary<byte, ePatternTech> dPatternTechs = new Dictionary<byte, ePatternTech>()
        {
            { 0x05, ePatternTech.LaserMatrixNBitWide },
            { 0x1E, ePatternTech.CIDKeyedFont },
            { 0x1F, ePatternTech.PFBType1 }
        };
        public ePatternTech PatternTech { get; private set; }
        public ushort MaxBoxWidth { get; private set; }
        public ushort MaxBoxHeight { get; private set; }
        public ushort FNORGLength { get; private set; }
        public ushort FNIRGLength { get; private set; }
        public ushort RasterDataCount { get; private set; }
        public ushort FNPRGLength { get; private set; }
        public ushort FNMRGLength { get; private set; }
        public ushort FNNRGLength { get; private set; }
        public uint FNNDataCount { get; private set; }

        public FNC(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        public override void ParseData()
        {
            base.ParseData();

            PatternTech = dPatternTechs[Data[1]];
            MaxBoxWidth = (ushort)GetNumericValue(GetSectionedData(10, 2), false);
            MaxBoxHeight = (ushort)GetNumericValue(GetSectionedData(12, 2), false);
            FNORGLength = (ushort)GetNumericValue(new[] { Data[14] }, false);
            FNIRGLength = (ushort)GetNumericValue(new[] { Data[15] }, false);
            RasterDataCount = (ushort)GetNumericValue(GetSectionedData(17, 2), false);
            FNPRGLength = (ushort)GetNumericValue(new[] { Data[20] }, false);
            FNMRGLength = (ushort)GetNumericValue(new[] { Data[21] }, false);
            if (Data.Length > 35)
                FNNRGLength = (ushort)GetNumericValue(new[] { Data[35] }, false);
            if (Data.Length > 39)
                FNNDataCount = (uint)GetNumericValue(GetSectionedData(36, 4), false);
        }
    }
}