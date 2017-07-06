using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IPO : StructuredField
	{
		private static string _abbr = "IPO";
		private static string _title = "Include Page Overlay";
		private static string _desc = "The Include Page Overlay structured field references an overlay resource definition that is to be positioned on the page.A page overlay can be referenced at any time during the page state, but not during an object state. The overlay contains its own active environment group definition. The current environment of the page that included the overlay is restored when the Include Page Overlay has been completed.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IPO(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}