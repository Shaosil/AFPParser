using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MSU : StructuredField
	{
		private static string _abbr = "MSU";
		private static string _title = "Map Suppression";
		private static string _desc = "The Map Suppression structured field maps one-byte text suppression local identifiers  to text suppression names.Suppressible text is identified in presentation text objects with a local identifier and is bracketed with control sequences that specify the beginning and the end of the suppression.  A text suppression is activated by specifying its local identifier in a Medium Modification Control (MMC) structured field in a medium map.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MSU(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}