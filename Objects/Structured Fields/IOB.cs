using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IOB : StructuredField
	{
		private static string _abbr = "IOB";
		private static string _title = "Include Object";
		private static string _desc = "An Include Object structured field references an object on a page or overlay. It optionally contains parameters that identify the object and that specify presentation parameters such as object position, size, orientation, mapping, and default color. Where the presentation parameters conflict with parameters specified in the object's environment group (OEG), the parameters in the Include Object structured field override. If the referenced object is a page segment, the IOB parameters override the corresponding environment group parameters on all data objects in the page segment.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IOB(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}