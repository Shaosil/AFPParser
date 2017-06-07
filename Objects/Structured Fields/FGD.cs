using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class FGD : StructuredField
	{
		private static string _abbr = "FGD";
		private static string _title = "Form Environment Group Descriptor (obsolete)";
		private static string _desc = "OBSOLETE STRUCTURED FIELD";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public FGD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}