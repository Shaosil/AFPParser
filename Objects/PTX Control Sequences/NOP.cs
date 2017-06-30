using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class NOP : PTXControlSequence
	{
		private static string _abbr = "NOP";
		private static string _desc = "No Operation";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Ignored Data")
        };

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public NOP(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}
