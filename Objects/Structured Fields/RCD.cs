using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class RCD : StructuredField
	{
		private static string _abbr = "RCD";
		private static string _title = "Record Descriptor";
		private static string _desc = "The Record Descriptor structured field contains information, such as record position, text orientation, font selection, field selection, and conditional processing identification, used to format line data that consists of records tagged with record identifiers.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public RCD(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}