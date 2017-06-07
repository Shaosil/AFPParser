using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IPS : StructuredField
	{
		private static string _abbr = "IPS";
		private static string _title = "Include Page Segment";
		private static string _desc = "The Include Page Segment structured field references a page segment resource object that is to be presented on the page or overlay presentation space.The IPS specifies a reference point on the including page or overlay coordinate system that may be used to position objects contained in the page segment.A page segment can be referenced at any time during page or overlay state, but not during an object state. The page segment inherits the active environment group definition of the including page or overlay.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IPS(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}