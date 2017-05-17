using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IDD : StructuredField
	{
		private static string _abbr = "IDD";
		private static string _title = "Image Data Descriptor IO";
		private static string _desc = "The Image Data Descriptor structured field contains the descriptor data for an image data object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public IDD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}