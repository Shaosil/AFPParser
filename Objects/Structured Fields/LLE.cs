using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class LLE : StructuredField
	{
		private static string _abbr = "LLE";
		private static string _title = "Link Logical Element";
		private static string _desc = "A Link Logical Element structured field specifies the linkage from a source document component to a target document component.The LLE identifies the source and target and indicates the purpose of the linkage by specifying a link type.The link source and link target may be in the same document component or in different document components, and they need not be of the same document component type.The linkage may involve a complete document component, or it may be restricted to a rectangular area on the presentation space associated with the document component. The Link Logical Element structured field can be embedded in the document that contains the link source, in the document that contains the link target, in the document index for either document, or in any combination of these structures. Link Logical Element parameters do not provide any presentation specifications.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public LLE(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}