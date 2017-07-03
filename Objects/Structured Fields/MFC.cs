using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MFC : StructuredField
	{
		private static string _abbr = "MFC";
		private static string _title = "Medium Finishing Control";
		private static string _desc = "The Medium Finishing Control structured field specifies the finishing requirements for physical media. Finishing can be specified for a media collection at the print file level or at the document level by placing the MFC in the document environment group(DEG) of the form map.Finishing can be specified for a media collection at the medium map level by placing the MFC in a medium map.Finishing can be specified for individual media, or sheets, at the medium map level by placing the MFC in a medium map.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MFC(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}