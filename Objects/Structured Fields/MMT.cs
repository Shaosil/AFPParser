using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MMT : StructuredField
	{
		private static string _abbr = "MMT";
		private static string _title = "Map Media Type";
		private static string _desc = "The Map Media Type structured field maps a media type local ID to the name or OID of a media type.See 'Media Type Identifiers' on page 639 for a list of media types registered by their name and their OID.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MMT(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}