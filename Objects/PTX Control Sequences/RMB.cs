using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class RMB : PTXControlSequence
	{
		private static string _abbr = "RMB";
		private static string _desc = "Relative Move Baseline";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public RMB(byte[] data) : base(data) { }
	}
}