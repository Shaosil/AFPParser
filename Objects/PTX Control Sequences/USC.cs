using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class USC : PTXControlSequence
	{
		private static string _abbr = "USC";
		private static string _desc = "Underscore";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public USC(byte[] data) : base(data) { }
	}
}
