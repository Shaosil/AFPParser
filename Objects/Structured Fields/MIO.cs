using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MIO : StructuredField
	{
		private static string _abbr = "MIO";
		private static string _title = "Map IO Image Object";
		private static string _desc = "The Map Image Object structured field specifies how an image data object is mapped into its object area.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public MIO(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}