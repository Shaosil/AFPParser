using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MMO : StructuredField
	{
		private static string _abbr = "MMO";
		private static string _title = "Map Medium Overlay";
		private static string _desc = "The Map Medium Overlay structured field maps one-byte medium overlay local identifiers that are specified by keywords in the Medium Modification Control (MMC) structured field to medium overlay names.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MMO(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}