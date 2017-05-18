using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class RPS : PTXControlSequence
	{
		private static string _abbr = "RPS";
		private static string _desc = "Repeat String";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public RPS(byte[] data) : base(data) { }
	}
}
