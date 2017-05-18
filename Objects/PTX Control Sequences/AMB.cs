using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class AMB : PTXControlSequence
	{
		private static string _abbr = "AMB";
		private static string _desc = "Absolute Move Baseline";
        private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public AMB(byte[] data) : base(data) { }
	}
}