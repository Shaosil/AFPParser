using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class FNN : StructuredField
	{
		private static string _abbr = "FNN";
		private static string _title = "Font Names (Outline Fonts Only)";
		private static string _desc = "";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public FNN(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}