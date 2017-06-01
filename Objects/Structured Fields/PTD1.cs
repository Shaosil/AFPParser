using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PTD1 : StructuredField
	{
		private static string _abbr = "PTD";
		private static string _title = "Presentation Text Descriptor (Format 1)";
		private static string _desc = "Specifies the size of a text object presentation space and the measurement units used for size and for all linear measurements within the text object.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CODE, "X Axis Unit Base") { Mappings = Lookups.CommonMappings.AxisBase },
			new Offset(1, Lookups.DataTypes.CODE, "Y Axis Unit Base") { Mappings = Lookups.CommonMappings.AxisBase },
			new Offset(2, Lookups.DataTypes.UBIN, "X Units Per X Unit Base"),
			new Offset(4, Lookups.DataTypes.UBIN, "Y Units Per Y Unit Base"),
			new Offset(6, Lookups.DataTypes.UBIN, "X Axis Space Extent"),
			new Offset(8, Lookups.DataTypes.UBIN, "Y Axis Space Extent"),
			new Offset(10, Lookups.DataTypes.EMPTY, "")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public PTD1(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}