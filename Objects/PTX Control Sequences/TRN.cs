using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class TRN : PTXControlSequence
	{
		private static string _abbr = "TRN";
		private static string _desc = "Transparent Data";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Data")
        };

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

		public TRN(byte[] data) : base(data) { }
	}
}
