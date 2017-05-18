using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class GIR : PTXControlSequence
	{
		private static string _abbr = "GIR";
		private static string _desc = "Glyph ID Run";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public GIR(byte[] data) : base(data) { }
	}
}
