using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BDG : StructuredField
	{
		private static string _abbr = "BDG";
		private static string _title = "Begin Document Environment Group";
		private static string _desc = "The Begin Document Environment Group structured field begins a document environment group, which establishes the environment parameters for the form map object. The scope of the document environment group is the containing form map.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BDG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}