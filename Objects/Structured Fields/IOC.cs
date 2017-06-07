using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IOC : StructuredField
	{
		private static string _abbr = "IOC";
		private static string _title = "Image Output Control IM";
		private static string _desc = "The IM Image Output Control structured field specifies the position and orientation of the IM image object area and the mapping of the image points to presentation device pels.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IOC(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}