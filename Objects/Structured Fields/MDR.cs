using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MDR : StructuredField
	{
		private static string _abbr = "MDR";
		private static string _title = "Map Data Resource";
		private static string _desc = "Uses repeating groups to specify resources that are required for presentation.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""), // Repeating group length
            new Offset(2, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => true;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MDR(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}