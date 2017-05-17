using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class GDD : StructuredField
	{
		private static string _abbr = "GDD";
		private static string _title = "Graphics Data Descriptor";
		private static string _desc = "The Graphics Data Descriptor structured field contains the descriptor data for a graphics object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public GDD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}