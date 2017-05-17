using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BII : StructuredField
	{
		private static string _abbr = "BII";
		private static string _title = "Begin Image Object IM";
		private static string _desc = "The Begin IM Image Object structured field begins an IM image data object, which becomes the current data object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BII(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}