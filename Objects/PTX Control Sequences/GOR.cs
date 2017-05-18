using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class GOR : PTXControlSequence
	{
		private static string _abbr = "GOR";
		private static string _desc = "Glyph Offset Run";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public GOR(byte[] data) : base(data) { }
	}
}
