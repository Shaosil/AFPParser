using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EOG : StructuredField
	{
		private static string _abbr = "EOG";
		private static string _title = "End Object Environment Group";
		private static string _desc = "The End Object Environment Group structured field terminates the definition of an Object Environment Group initiated by a Begin Object Environment Group structured field.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Object Environment Group Name")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EOG(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}