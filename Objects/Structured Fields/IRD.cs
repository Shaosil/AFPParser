using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IRD : StructuredField
	{
		private static string _abbr = "IRD";
		private static string _title = "Image Raster Data IM";
		private static string _desc = "The IM Image Raster Data structured field contains the image points that define the raster pattern for an IM image data object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IRD(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}