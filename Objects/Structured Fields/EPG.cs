using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EPG : StructuredField
	{
		private static string _abbr = "EPG";
		private static string _title = "End Page";
		private static string _desc = "The End Page structured field terminates the current presentation page definition initiated by a Begin Page structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public EPG(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}