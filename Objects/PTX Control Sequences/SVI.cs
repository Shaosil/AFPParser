using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SVI : PTXControlSequence
	{
		private static string _abbr = "SVI";
		private static string _desc = "Set Variable Space Character Increment";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Increment")
        };

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int Increment { get; private set; }

		public SVI(byte id, byte[] sequence, byte[] data) : base(id, sequence, data) { }

        public override void ParseData()
        {
            // Don't set if FFFF
            Increment = (int)GetNumericValue(GetSectionedData(0, 2), true);
            if (Increment < 0) Increment = 0;
        }
    }
}