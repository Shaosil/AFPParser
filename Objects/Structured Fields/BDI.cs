using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BDI : StructuredField
	{
		private static string _abbr = "BDI";
		private static string _title = "Begin Document Index";
		private static string _desc = "The Begin Document Index structured field begins the document index.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BDI(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}