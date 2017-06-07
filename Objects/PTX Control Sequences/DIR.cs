using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class DIR : PTXControlSequence
	{
		private static string _abbr = "DIR";
		private static string _desc = "Draw I-axis Rule";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public DIR(byte[] data) : base(data) { }
	}
}
