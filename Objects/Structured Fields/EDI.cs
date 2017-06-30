using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EDI : StructuredField
	{
		private static string _abbr = "EDI";
		private static string _title = "End Document Index";
		private static string _desc = "The End Document Index structured field terminates the document index initiated by a Begin Document Index structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EDI(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}