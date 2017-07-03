using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class DBR : PTXControlSequence
	{
		private static string _abbr = "DBR";
		private static string _desc = "Draw B-axis Rule";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Length"),
            new Offset(2, Lookups.DataTypes.SBIN, "Width"),
            new Offset(4, Lookups.DataTypes.EMPTY, "") // Actually the frational specifier byte of the width, but we probably don't need to support that
        };

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int RuleLength { get; private set; }
        public int RuleWidth { get; private set; }

        public DBR(byte id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override void ParseData()
        {
            RuleLength = (int)GetNumericValue(GetSectionedData(0, 2), true);
            RuleWidth = (int)GetNumericValue(GetSectionedData(2, 2), true);
        }
    }
}
