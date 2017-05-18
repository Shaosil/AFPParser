using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class BSU : PTXControlSequence
	{
		private static string _abbr = "BSU";
		private static string _desc = "Begin Suppression";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public BSU(byte[] data) : base(data) { }
	}
}
