using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class NOP : PTXControlSequence
	{
		private static string _abbr = "NOP";
		private static string _desc = "No Operation";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public NOP(byte[] data) : base(data) { }
	}
}
