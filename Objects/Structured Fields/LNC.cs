using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class LNC : StructuredField
	{
		private static string _abbr = "LNC";
		private static string _title = "Line Descriptor Count";
		private static string _desc = "The Line Descriptor Count structured field specifies the number of Line Descriptor (LND), Record Descriptor (RCD) or XML Descriptor (XMD) structured fields in the Data Map Transmission Subcase.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public LNC(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}