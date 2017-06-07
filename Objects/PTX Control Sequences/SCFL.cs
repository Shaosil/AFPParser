using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SCFL : PTXControlSequence
	{
		private static string _abbr = "SCFL";
		private static string _desc = "Set Coded Font Local";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Identifier")
        };

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public SCFL(byte[] data) : base(data) { }
	}
}
