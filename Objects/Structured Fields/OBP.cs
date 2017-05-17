using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class OBP : StructuredField
	{
		private static string _abbr = "OBP";
		private static string _title = "Object Area Position";
		private static string _desc = "The Object Area Position structured field specifies the origin and orientation of the object area, and the origin and orientation of the object content within the object area.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public OBP(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}