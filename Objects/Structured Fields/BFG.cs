using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BFG : StructuredField
	{
		private static string _abbr = "BFG";
		private static string _title = "Begin Form Environment Group (obsolete)";
		private static string _desc = "OBSOLETE FIELD";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BFG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}