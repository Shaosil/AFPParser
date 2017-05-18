using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class DBR : PTXControlSequence
	{
		private static string _abbr = "DBR";
		private static string _desc = "Draw B-axis Rule";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public DBR(byte[] data) : base(data) { }
	}
}
