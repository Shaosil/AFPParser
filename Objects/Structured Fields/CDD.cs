using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class CDD : StructuredField
	{
		private static string _abbr = "CDD";
		private static string _title = "Container Data Descriptor";
		private static string _desc = "The Container Data Descriptor structured field specifies control information for a presentation data object that is carried in an object container.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public CDD(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}