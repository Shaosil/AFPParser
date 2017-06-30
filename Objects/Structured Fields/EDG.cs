using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EDG : StructuredField
	{
		private static string _abbr = "EDG";
		private static string _title = "End Document Environment Group";
		private static string _desc = "The End Document Environment Group structured field terminates the definition of a document environment group initiated by a Begin Document Environment Group structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EDG(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}