using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BDM : StructuredField
	{
		private static string _abbr = "BDM";
		private static string _title = "Begin Data Map";
		private static string _desc = "The Begin Data Map structured field begins a Data Map resource object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BDM(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}