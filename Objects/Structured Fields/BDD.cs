using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BDD : StructuredField
	{
		private static string _abbr = "BDD";
		private static string _title = "Bar Code Data Descriptor";
		private static string _desc = "The Bar Code Data Descriptor structured field contains the descriptor data for a bar code data object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BDD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}