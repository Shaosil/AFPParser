using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SIA : PTXControlSequence
	{
		private static string _abbr = "SIA";
		private static string _desc = "Set Intercharacter Adjustment";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public SIA(byte[] data) : base(data) { }
	}
}