using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BOG : StructuredField
	{
		private static string _abbr = "BOG";
		private static string _title = "Begin Object Environment Group";
		private static string _desc = "The Begin Object Environment Group structured field begins an Object Environment Group, which establishes the environment parameters for the object.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Object Environment Group Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BOG(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}