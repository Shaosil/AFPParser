using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BPS : StructuredField
	{
		private static string _abbr = "BPS";
		private static string _title = "Begin Page Segment";
		private static string _desc = "Begins a page segment, which is a referenced resource object that can contain Bar Codes, Graphics, and Images.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Page Segment Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BPS(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}