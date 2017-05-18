using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SVI : PTXControlSequence
	{
		private static string _abbr = "SVI";
		private static string _desc = "Set Variable Space Character Increment";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public SVI(byte[] data) : base(data) { }
	}
}