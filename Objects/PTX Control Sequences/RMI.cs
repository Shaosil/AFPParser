using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class RMI : PTXControlSequence
	{
		private static string _abbr = "RMI";
		private static string _desc = "Relative Move Inline";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public RMI(byte[] data) : base(data) { }
	}
}