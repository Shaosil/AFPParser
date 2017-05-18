using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SIA : PTXControlSequence
	{
		private static string _abbr = "SIA";
		private static string _desc = "Set Intercharacter Adjustment";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public SIA(byte[] data) : base(data) { }
	}
}