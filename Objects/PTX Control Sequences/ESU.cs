using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class ESU : PTXControlSequence
	{
		private static string _abbr = "ESU";
		private static string _desc = "End Suppression";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public ESU(byte[] data) : base(data) { }
	}
}
