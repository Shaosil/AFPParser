using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MBC : StructuredField
	{
		private static string _abbr = "MBC";
		private static string _title = "Map Bar Code";
		private static string _desc = "The Map Bar Code Object structured field specifies how a bar code data object is to be mapped into its object area.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MBC(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}