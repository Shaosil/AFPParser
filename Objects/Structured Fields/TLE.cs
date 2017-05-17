using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class TLE : StructuredField
	{
		private static string _abbr = "TLE";
		private static string _title = "Tag Logical Element";
		private static string _desc = "Assigns an attribute name and value to a page or group.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.TRIPS, "")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public TLE(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}