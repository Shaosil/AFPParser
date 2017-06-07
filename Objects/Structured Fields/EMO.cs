using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EMO : StructuredField
	{
		private static string _abbr = "EMO";
		private static string _title = "End Overlay";
		private static string _desc = "The End Overlay structured field terminates the overlay resource object initiated by a Begin Overlay structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EMO(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}