using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class ESU : PTXControlSequence
	{
		private static string _abbr = "ESU";
		private static string _desc = "End Suppression";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public ESU(byte[] data) : base(data) { }
	}
}
