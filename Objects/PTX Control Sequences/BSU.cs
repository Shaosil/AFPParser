using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class BSU : PTXControlSequence
	{
		private static string _abbr = "BSU";
		private static string _desc = "Begin Suppression";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BSU(byte[] data) : base(data) { }
	}
}
