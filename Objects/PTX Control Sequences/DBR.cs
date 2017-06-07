using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class DBR : PTXControlSequence
	{
		private static string _abbr = "DBR";
		private static string _desc = "Draw B-axis Rule";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public DBR(byte[] data) : base(data) { }
	}
}
