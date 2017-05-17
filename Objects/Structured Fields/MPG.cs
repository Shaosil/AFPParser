using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MPG : StructuredField
	{
		private static string _abbr = "MPG";
		private static string _title = "Map Page";
		private static string _desc = "The Map Page structured field identifies a page that is to be merged with data specified for the current page by using an Include Page(IPG) structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public MPG(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}