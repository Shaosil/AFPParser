using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PGP1 : StructuredField
	{
		private static string _abbr = "PGP";
		private static string _title = "Page Position (Format 1)";
		private static string _desc = "Specifies the position and orientation of a page's presentation space on the medium presentation space for the physical medium.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "X Origin"),
            new Offset(3, Lookups.DataTypes.UBIN, "Y Origin")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public PGP1(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}