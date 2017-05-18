using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class OVS : PTXControlSequence
	{
		private static string _abbr = "OVS";
		private static string _desc = "Overstrike";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public OVS(byte[] data) : base(data) { }
	}
}
