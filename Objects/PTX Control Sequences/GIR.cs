using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class GIR : PTXControlSequence
	{
		private static string _abbr = "GIR";
		private static string _desc = "Glyph ID Run";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public GIR(byte[] data) : base(data) { }
	}
}
