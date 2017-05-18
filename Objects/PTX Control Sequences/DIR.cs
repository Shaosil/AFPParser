using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class DIR : PTXControlSequence
	{
		private static string _abbr = "DIR";
		private static string _desc = "Draw I-axis Rule";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public DIR(byte[] data) : base(data) { }
	}
}
