using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class STC : PTXControlSequence
	{
		private static string _abbr = "STC";
		private static string _desc = "Set Text Color";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public STC(byte[] data) : base(data) { }
	}
}
