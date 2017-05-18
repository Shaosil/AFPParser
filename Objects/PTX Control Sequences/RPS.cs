using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class RPS : PTXControlSequence
	{
		private static string _abbr = "RPS";
		private static string _desc = "Repeat String";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public RPS(byte[] data) : base(data) { }
	}
}
