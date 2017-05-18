using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class UCT : PTXControlSequence
	{
		private static string _abbr = "UCT";
		private static string _desc = "Unicode Complex Text";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public UCT(byte[] data) : base(data) { }
	}
}
