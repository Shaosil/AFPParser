using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MPO : StructuredField
	{
		private static string _abbr = "MPO";
		private static string _title = "Map Page Overlay";
		private static string _desc = "The Map Page Overlay structured field maps local identifiers to page overlay names.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MPO(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}