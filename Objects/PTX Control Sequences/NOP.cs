using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class NOP : PTXControlSequence
	{
		private static string _abbr = "NOP";
		private static string _desc = "No Operation";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public NOP(byte[] data) : base(data) { }
	}
}
