using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IMM : StructuredField
	{
		private static string _abbr = "IMM";
		private static string _title = "Invoke Medium Map";
		private static string _desc = "Identifies the medium map that is to become active for the document, and affects the current environment until a new map is invoked.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CHAR, "Name"),
			new Offset(8, Lookups.DataTypes.TRIPS, "")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public IMM(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}