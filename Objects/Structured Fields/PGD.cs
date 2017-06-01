using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PGD : StructuredField
	{
		private static string _abbr = "PGD";
		private static string _title = "Page Descriptor";
		private static string _desc = "Specifies the size and attributes of a page or overlay presentation space.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CODE, "X Axis Base") { Mappings = Lookups.CommonMappings.AxisBase },
			new Offset(1, Lookups.DataTypes.CODE, "Y Axis Base") { Mappings = Lookups.CommonMappings.AxisBase },
			new Offset(2, Lookups.DataTypes.UBIN, "Units Per X Base"),
			new Offset(4, Lookups.DataTypes.UBIN, "Units Per Y Base"),
			new Offset(6, Lookups.DataTypes.UBIN, "Page X Size"),
			new Offset(9, Lookups.DataTypes.UBIN, "Page Y Size"),
			new Offset(12, Lookups.DataTypes.EMPTY, ""),
			new Offset(15, Lookups.DataTypes.TRIPS, "")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public PGD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}