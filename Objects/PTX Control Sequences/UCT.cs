using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class UCT : PTXControlSequence
	{
		private static string _abbr = "UCT";
		private static string _desc = "Unicode Complex Text";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public UCT(byte[] data) : base(data) { }
	}
}
