using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BSG : StructuredField
	{
		private static string _abbr = "BSG";
		private static string _title = "Begin Resource Environment Group";
		private static string _desc = "The Begin Resource Environment Group structured field begins a Resource Environment Group(REG), which defines a subset of the resources required for a document or for a group of pages in a document.The scope of the Resource Environment Group is the group of pages that follow, up to the next REG, which is a complete replacement for the current REG, or the end of the document, whichever occurs first.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BSG(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}