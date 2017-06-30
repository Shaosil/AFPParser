using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BOC : StructuredField
	{
		private static string _abbr = "BOC";
		private static string _title = "Begin Object Container";
		private static string _desc = "The Begin Object Container structured field begins an object container, which may be used to envelop and carry object data.The object data may or may not bedefined by an AFP presentation architecture.";
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

		public BOC(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}