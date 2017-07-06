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
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EFG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}