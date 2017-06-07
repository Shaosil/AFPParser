using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EFM : StructuredField
	{
		private static string _abbr = "EFM";
		private static string _title = "End Form Map";
		private static string _desc = "The End Form Map structured field terminates the form map object initiated by a Begin Form Map structured field";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EFM(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}