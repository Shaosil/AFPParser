using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class ICP : StructuredField
	{
		private static string _abbr = "ICP";
		private static string _title = "Image Cell Position";
		private static string _desc = "The IM Image Cell Position structured field specifies the placement, size, and replication of IM image cells.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "X Offset"),
            new Offset(2, Lookups.DataTypes.UBIN, "Y Offset"),
            new Offset(4, Lookups.DataTypes.UBIN, "X Size"),
            new Offset(6, Lookups.DataTypes.UBIN, "Y Size"),
            new Offset(8, Lookups.DataTypes.UBIN, "Size of X Fill Rectangle"),
            new Offset(10, Lookups.DataTypes.UBIN, "Size of Y Fill Rectangle")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }
        public int XSize { get; private set; }
        public int YSize { get; private set; }

        public ICP(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override void ParseData()
        {
            XOffset = (int)GetNumericValue(GetSectionedData(0, 2), false);
            YOffset = (int)GetNumericValue(GetSectionedData(2, 2), false);
            XSize = (int)GetNumericValue(GetSectionedData(4, 2), false);
            YSize = (int)GetNumericValue(GetSectionedData(6, 2), false);
        }
    }
}