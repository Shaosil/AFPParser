using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class FND : StructuredField
    {
        private static string _abbr = "FND";
        private static string _title = "Font Descriptor";
        private static string _desc = "Specifies the overall characteristics of a font character set.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Typeface Description"),
            new Offset(32, Lookups.DataTypes.CODE, "Weight Class") { Mappings = CommonMappings.WeightClass },
            new Offset(33, Lookups.DataTypes.CODE, "Width Class") { Mappings = CommonMappings.WidthClass },
            new Offset(34, Lookups.DataTypes.UBIN, "Max Vertical Size"),
            new Offset(36, Lookups.DataTypes.UBIN, "Nominal Vertical Size"),
            new Offset(38, Lookups.DataTypes.UBIN, "Min Vertical Size"),
            new Offset(40, Lookups.DataTypes.UBIN, "Max Horizontal Size"),
            new Offset(42, Lookups.DataTypes.UBIN, "Nominal Horizontal Size"),
            new Offset(44, Lookups.DataTypes.UBIN, "Min Horizontal Size"),
            new Offset(46, Lookups.DataTypes.UBIN, "Design General Class"),
            new Offset(47, Lookups.DataTypes.UBIN, "Design Subclass"),
            new Offset(48, Lookups.DataTypes.UBIN, "Design Specific Group"),
            new Offset(49, Lookups.DataTypes.EMPTY, ""),
            new Offset(64, Lookups.DataTypes.BITS, "Font Design Flags") { Mappings = CommonMappings.FontDesignFlags },
            new Offset(66, Lookups.DataTypes.EMPTY, ""),
            new Offset(76, Lookups.DataTypes.UBIN, "Graphic Character Set GID"),
            new Offset(78, Lookups.DataTypes.UBIN, "Font Typeface GID"),
            new Offset(80, Lookups.DataTypes.TRIPS, ""),
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int NominalVerticalSize { get; private set; }
        public int NominalHorizontalSize { get; private set; }
        public float EmInches => (NominalVerticalSize / 10f) / 72;
        public float EmXInches => (NominalHorizontalSize / 20f) / 72;

        public FND(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        public override void ParseData()
        {
            NominalVerticalSize = (int)GetNumericValue(GetSectionedData(36, 2), false);
            NominalHorizontalSize = (int)GetNumericValue(GetSectionedData(42, 2), false);
        }
    }
}