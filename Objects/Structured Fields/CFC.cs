using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class CFC : StructuredField
	{
		private static string _abbr = "CFC";
		private static string _title = "Coded Font Control";
		private static string _desc = "";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public CFC(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}