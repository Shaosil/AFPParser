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
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public DXD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}