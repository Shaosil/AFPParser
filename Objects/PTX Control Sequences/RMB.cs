using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class RMB : PTXControlSequence
	{
		private static string _abbr = "RMB";
		private static string _desc = "Relative Move Baseline";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public RMB(byte[] data) : base(data) { }
	}
}