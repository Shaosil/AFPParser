using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PEC : StructuredField
	{
		private static string _abbr = "PEC";
		private static string _title = "Presentation Environment Control";
		private static string _desc = "The Presentation Environment Control structured field specifies parameters that affect the rendering of presentation data and the appearance that is to be assumed by the presentation device.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public PEC(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}