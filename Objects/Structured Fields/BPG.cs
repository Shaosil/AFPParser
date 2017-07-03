using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BPG : StructuredField
	{
		private static string _abbr = "BPG";
		private static string _title = "Begin Page";
		private static string _desc = "Begins a presentation page, which contains an active environment group used to establish parameters.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CHAR, "Page Name"),
			new Offset(8, Lookups.DataTypes.TRIPS, "")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BPG(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}