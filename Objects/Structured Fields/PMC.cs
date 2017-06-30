using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PMC : StructuredField
	{
		private static string _abbr = "PMC";
		private static string _title = "Page Modification Control";
		private static string _desc = "The Page Modification Control structured field specifies modifications to be applied to a page presented on a physical medium. If the ID of a specific PMC is selected in the PGP structured field of the active medium map in N-up mode, only the modifications specified by that PMC are applied to pages placed on the medium.If a specific PMC is not selected in N-up mode, all modifications specified by all PMCs in the active medium map are applied to pages placed on the medium.";
        private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public PMC(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}