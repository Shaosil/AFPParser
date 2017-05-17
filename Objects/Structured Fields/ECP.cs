using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class ECP : StructuredField
	{
		private static string _abbr = "ECP";
		private static string _title = "End Code Page";
		private static string _desc = "";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public ECP(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}