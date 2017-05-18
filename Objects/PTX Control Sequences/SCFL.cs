using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SCFL : PTXControlSequence
	{
		private static string _abbr = "SCFL";
		private static string _desc = "Set Coded Font Local";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, "Identifier")
        };

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

        public SCFL(byte[] data) : base(data) { }
	}
}
