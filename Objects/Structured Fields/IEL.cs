using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IEL : StructuredField
	{
		private static string _abbr = "IEL";
		private static string _title = "Index Element";
		private static string _desc = "The Index Element structured field identifies begin structured fields for use within a document index.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IEL(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}