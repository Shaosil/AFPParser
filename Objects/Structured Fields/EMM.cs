using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EMM : StructuredField
	{
		private static string _abbr = "EMM";
		private static string _title = "End Medium Map";
		private static string _desc = "Terminates the medium map object initiated by a Begin Medium Map structured field";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Medium Map Name")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EMM(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}