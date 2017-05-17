using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BNG : StructuredField
	{
		private static string _abbr = "BNG";
		private static string _title = "Begin Named Page Group";
		private static string _desc = "Begins a page group, which is a named logical grouping of sequential pages. A page group may contain nested page groups.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CHAR, "Name"),
			new Offset(8, Lookups.DataTypes.TRIPS, "")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BNG(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}