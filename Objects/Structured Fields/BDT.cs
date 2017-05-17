using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BDT : StructuredField
	{
		private static string _abbr = "BDT";
		private static string _title = "Begin Document";
		private static string _desc = "The Begin Document structured field names and begins the document.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CHAR, "Name"),
			new Offset(8, Lookups.DataTypes.EMPTY, ""),
			new Offset(10, Lookups.DataTypes.TRIPS, "")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BDT(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}