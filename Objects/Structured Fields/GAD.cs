using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class GAD : StructuredField
	{
		private static string _abbr = "GAD";
		private static string _title = "Graphics Data";
		private static string _desc = "The Graphics Data structured field contains the data for a graphics object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public GAD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}