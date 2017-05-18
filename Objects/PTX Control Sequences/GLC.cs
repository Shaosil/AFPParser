using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class GLC : PTXControlSequence
	{
		private static string _abbr = "GLC";
		private static string _desc = "Glyph Layout Control";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public GLC(byte[] data) : base(data) { }
	}
}
