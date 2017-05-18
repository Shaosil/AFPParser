using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SBI : PTXControlSequence
	{
		private static string _abbr = "SBI";
		private static string _desc = "Set Baseline Increment";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public SBI(byte[] data) : base(data) { }
	}
}