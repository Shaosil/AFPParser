using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BPS : StructuredField
	{
		private static string _abbr = "BPS";
		private static string _title = "Begin Page Segment";
		private static string _desc = "The Begin Page Segment structured field begins a page segment. A page segment is a resource object that can be referenced from a page or overlay and that contains any mixture of: Bar code objects(BCOCA), Graphics objects(GOCA), v Image objects(IOCA). Objects in a page segment must specify an object area offset of zero so that they are positioned either at the origin of the including page or overlay coordinate system or at a reference point that is defined on the including page or overlay coordinate system by the Include Page Segment(IPS) structured field. A page segment does not contain an active environment group.The environment for a page segment is defined by the active environment group of the including page or overlay.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BPS(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}