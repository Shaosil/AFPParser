using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class GAR : PTXControlSequence
	{
		private static string _abbr = "GAR";
		private static string _desc = "Glyph Advance Run";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public GAR(byte[] data) : base(data) { }
	}
}
