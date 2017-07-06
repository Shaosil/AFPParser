using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IPG : StructuredField
	{
		private static string _abbr = "IPG";
		private static string _title = "Include Page";
		private static string _desc = "The Include Page structured field references a page that is to be included in the document. The Include Page structured field may occur in document state, page-group state, or page state.In all three cases the referenced page is positioned on the media using the (Xm, Ym) offsets specified in the PGP structured field in the active medium map.The referenced page must not contain another Include Page structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IPG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}