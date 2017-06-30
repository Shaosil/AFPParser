using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BRS : StructuredField
	{
		private static string _abbr = "BRS";
		private static string _title = "Begin Resource";
		private static string _desc = "Begins an envelope that is used to carry resource objects in print file level resource groups.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CHAR, "Identifier"),
			new Offset(8, Lookups.DataTypes.EMPTY, ""),
			new Offset(10, Lookups.DataTypes.TRIPS, "")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BRS(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}