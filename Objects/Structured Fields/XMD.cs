using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class XMD : StructuredField
	{
		private static string _abbr = "XMD";
		private static string _title = "XML Descriptor";
		private static string _desc = "The XML Descriptor structured field contains information, such as data position, text orientation, font selection, field selection, and conditional processing identification, used to format XML data that consists of text delimited by start and end tags. Note: The XMDs in a Data Map are numbered sequentially, starting with 1.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public XMD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}