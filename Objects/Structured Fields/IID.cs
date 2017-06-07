using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IID : StructuredField
	{
		private static string _abbr = "IID";
		private static string _title = "Image Input Descriptor IM";
		private static string _desc = "The IM Image Input Descriptor structured field contains the descriptor data for an IM image data object. This data specifies the resolution, size, and color of the IM image.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IID(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}