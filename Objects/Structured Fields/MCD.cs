using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MCD : StructuredField
	{
		private static string _abbr = "MCD";
		private static string _title = "Map Container Data";
		private static string _desc = "The Map Container Data structured field specifies how a presentation data object that is carried within an object container is mapped into its object area.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MCD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}