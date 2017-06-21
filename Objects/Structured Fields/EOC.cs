using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EOC : StructuredField
	{
		private static string _abbr = "EOC";
		private static string _title = "End Object Container";
		private static string _desc = "The End Object Container structured field terminates an object container initiated by a Begin Object Container structured field.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Object Container Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EOC(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}