using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EMM : StructuredField
	{
		private static string _abbr = "EMM";
		private static string _title = "End Medium Map";
		private static string _desc = "The End Medium Map structured field terminates the medium map object initiated by a Begin Medium Map structured field";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public EMM(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}