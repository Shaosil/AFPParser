using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class USC : PTXControlSequence
	{
		private static string _abbr = "USC";
		private static string _desc = "Underscore";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public USC(byte[] data) : base(data) { }
	}
}
