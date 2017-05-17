using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EFG : StructuredField
	{
		private static string _abbr = "EFG";
		private static string _title = "End Form Environment Group (obsolete)";
		private static string _desc = "Documentation for this does not have a description...";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public EFG(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}