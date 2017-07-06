using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EPS : StructuredField
	{
		private static string _abbr = "EPS";
		private static string _title = "End Page Segment";
		private static string _desc = "The End Page Segment structured field terminates the page segment resource object initiated by a Begin Page Segment structured field.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Page Segment Name")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EPS(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}