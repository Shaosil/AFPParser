using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class TBM : PTXControlSequence
	{
		private static string _abbr = "TBM";
		private static string _desc = "Temporary Baseline Move";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public TBM(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}
