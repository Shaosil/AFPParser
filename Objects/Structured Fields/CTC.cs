using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class CTC : StructuredField
	{
		private static string _abbr = "CTC";
		private static string _title = "Composed Text Control (obsolete)";
		private static string _desc = "OBSOLETE STRUCTURED FIELD";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public CTC(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}