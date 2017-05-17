using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MMC : StructuredField
	{
		private static string _abbr = "MMC";
		private static string _title = "Medium Modification Control";
		private static string _desc = "The Medium Modification Control structured field specifies the medium modifications to be applied for a copy subgroup specified in the Medium Copy Count(MCC) structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public MMC(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}