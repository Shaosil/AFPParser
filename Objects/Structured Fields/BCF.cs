using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BCF : StructuredField
	{
		private static string _abbr = "BCF";
		private static string _title = "Begin Coded Font";
		private static string _desc = "Begins a coded font object.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Coded Font Name")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BCF(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}