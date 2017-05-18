using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class GAR : PTXControlSequence
	{
		private static string _abbr = "GAR";
		private static string _desc = "Glyph Advance Run";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public GAR(byte[] data) : base(data) { }
	}
}
