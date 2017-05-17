using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PTD2 : StructuredField
	{
		private static string _abbr = "PTD";
		private static string _title = "Presentation Text Descriptor (Format 2)";
		private static string _desc = "The Presentation Text Data Descriptor structured field contains the descriptor data for a presentation text data object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public PTD2(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}