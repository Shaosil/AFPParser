using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class CCP : StructuredField
	{
		private static string _abbr = "CCP";
		private static string _title = "Conditional Processing Control";
		private static string _desc = "The Conditional Processing Control structured field defines tests to be performed on selected input records in line data and specifies the actions to take based on the test results. This optional structured field is selected with LND, RCD or XMD structured fields in the Page Definition. An LND, RCD or XMD can have a unique CCP associated with it or it can reference a CCP that has already been used. In either case, the CCP is referenced with the CCPID field of the LND, RCD or XMD. If a CCP structured field is included in a Page Definition, it must appear before the Data Maps in the Page Definition.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public CCP(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}