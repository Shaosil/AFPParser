using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EBC : StructuredField
	{
		private static string _abbr = "EBC";
		private static string _title = "End Bar Code Object";
		private static string _desc = "The End Bar Code Object structured field terminates the current bar code object initiated by a Begin Bar Code Object structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EBC(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}