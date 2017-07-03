using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BFM : StructuredField
	{
		private static string _abbr = "BFM";
		private static string _title = "Begin Form Map";
		private static string _desc = "Begins a form map object, also called a form definition or formdef.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Form Map Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BFM(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}