using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BDX : StructuredField
	{
		private static string _abbr = "BDX";
		private static string _title = "Begin Data Map Transmission Subcase";
		private static string _desc = "The Begin Data Map Transmission Subcase structured field begins a Data Map Transmission Subcase object, which contains the structured fields used to map lines of data to the page.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BDX(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}