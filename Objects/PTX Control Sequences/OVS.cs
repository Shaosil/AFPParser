using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class OVS : PTXControlSequence
	{
		private static string _abbr = "OVS";
		private static string _desc = "Overstrike";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public OVS(byte[] data) : base(data) { }
	}
}
