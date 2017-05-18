using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class BLN : PTXControlSequence
	{
		private static string _abbr = "BLN";
		private static string _desc = "Begin Line";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public BLN(byte[] data) : base(data) { }
	}
}
