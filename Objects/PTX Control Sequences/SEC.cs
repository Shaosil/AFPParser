using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SEC : PTXControlSequence
	{
		private static string _abbr = "SEC";
		private static string _desc = "Set Extended Text Color";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public SEC(byte[] data) : base(data) { }
	}
}
