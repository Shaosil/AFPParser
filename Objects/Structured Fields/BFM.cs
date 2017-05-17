using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BFM : StructuredField
	{
		private static string _abbr = "BFM";
		private static string _title = "Begin Form Map";
		private static string _desc = "The Begin Form Map structured field begins a form map object, also called a form definition or formdef.A form map is a print control resource object that contains one or more medium map resource objects that are invokable on document and page boundaries and that specify a complete set of presentation controls.It also contains an optional document environment group(DEG) that defines the presentation environment for the form map.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BFM(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}