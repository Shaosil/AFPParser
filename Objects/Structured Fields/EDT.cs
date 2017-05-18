using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EDT : StructuredField
	{
		private static string _abbr = "EDT";
		private static string _title = "End Document";
		private static string _desc = "The End Document structured field terminates the MO:DCA document data stream initiated by a Begin Document structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public EDT(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}