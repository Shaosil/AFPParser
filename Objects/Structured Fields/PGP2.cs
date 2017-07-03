using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PGP2 : StructuredField
	{
		private static string _abbr = "PGP";
		private static string _title = "Page Position (Format 2)";
		private static string _desc = "The Page Position structured field specifies the position and orientation of a page's presentation space on the medium presentation space for the physical medium. The PGP may be located in a medium map or in the document environment group of a form map.When present in the active medium map, it overrides a PGP in the document environment group of the form map. If N-up partitioning is specified by the Medium Modification Control structured field in the active medium map, the medium presentation spaces on the front and back sides of a sheet are divided into N partitions; and the Page Position structured field specifies the partition into which each page is mapped and with respect to which the page presentation space is positioned and oriented. Read More in docs";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public PGP2(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}