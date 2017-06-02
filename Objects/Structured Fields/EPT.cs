using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EPT : StructuredField
	{
		private static string _abbr = "EPT";
		private static string _title = "End Presentation Text";
		private static string _desc = "The End Presentation Text Object structured field terminates the current presentation text object initiated by a Begin Presentation Text Object structured field.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Presentation Text Data Object Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public EPT(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}