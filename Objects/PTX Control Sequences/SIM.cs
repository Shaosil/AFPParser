using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SIM : PTXControlSequence
	{
		private static string _abbr = "SIM";
		private static string _desc = "Set Inline Margin";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public SIM(byte[] data) : base(data) { }
	}
}