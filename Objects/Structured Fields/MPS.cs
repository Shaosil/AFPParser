using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MPS : StructuredField
	{
		private static string _abbr = "MPS";
		private static string _title = "Map Page Segment";
		private static string _desc = "The Map Page Segment structured field identifies page segments that are required to present a page on a physical medium.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MPS(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}