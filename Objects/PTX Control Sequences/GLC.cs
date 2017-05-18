using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class GLC : PTXControlSequence
	{
		private static string _abbr = "GLC";
		private static string _desc = "Glyph Layout Control";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public GLC(byte[] data) : base(data) { }
	}
}
