using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class TBM : PTXControlSequence
	{
		private static string _abbr = "TBM";
		private static string _desc = "Temporary Baseline Move";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public TBM(byte[] data) : base(data) { }
	}
}
