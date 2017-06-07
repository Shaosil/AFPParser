using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class ICP : StructuredField
	{
		private static string _abbr = "ICP";
		private static string _title = "Image Cell Position";
		private static string _desc = "The IM Image Cell Position structured field specifies the placement, size, and replication of IM image cells.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public ICP(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}