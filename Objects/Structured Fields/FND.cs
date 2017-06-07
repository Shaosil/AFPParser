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
            new Offset(32, Lookups.DataTypes.CODE, "Weight Class")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x01, "Ultralight" },
                    { 0x02, "Extralight" },
                    { 0x03, "Light" },
                    { 0x04, "Semilight" },
                    { 0x05, "Medium (normal)" },
                    { 0x06, "Semibold" },
                    { 0x07, "Bold" },
                    { 0x08, "Extrabold" },
                    { 0x09, "Ultrabold" }
                }
            },
            new Offset(33, Lookups.DataTypes.CODE, "Width Class")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x01, "Ultracondensed" },
                    { 0x02, "Extracondensed" },
                    { 0x03, "Condensed" },
                    { 0x04, "Semicondensed" },
                    { 0x05, "Medium (normal)" },
                    { 0x06, "Semiexpanded" },
                    { 0x07, "Expanded" },
                    { 0x08, "Extraexpanded" },
                    { 0x09, "Ultraexpanded" }
                }
            },
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
            new Offset(64, Lookups.DataTypes.BITS, "Font Design Flags")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Not Italic|Italic" },
                    { 0x01, "Not Underscored|Underscored" },
                    { 0x03, "Solid|Hollow" },
                    { 0x04, "Not Overstruck|Overstruck" }
                }
            },
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

		public FND(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}