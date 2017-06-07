using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class FDS : StructuredField
	{
		private static string _abbr = "FDS";
		private static string _title = "Fixed Data Size";
		private static string _desc = "The Fixed Data Size structured field specifies the number of bytes of text found in the following Fixed Data Text (FDX) structured fields.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public FDS(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}