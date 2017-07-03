using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class DXD : StructuredField
	{
		private static string _abbr = "DXD";
		private static string _title = "Data Map Transmission Subcase Descriptor";
		private static string _desc = "The Data Map Transmission Subcase Descriptor structured field is supported only for migration purposes.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public DXD(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}