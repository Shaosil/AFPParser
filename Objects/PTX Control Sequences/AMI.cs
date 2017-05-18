using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class AMI : PTXControlSequence
	{
		private static string _abbr = "AMI";
		private static string _desc = "Absolute Move Inline";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public AMI(byte[] data) : base(data) { }
	}
}