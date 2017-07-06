using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class ENG : StructuredField
	{
		private static string _abbr = "ENG";
		private static string _title = "End Named Page Group";
		private static string _desc = "The End Named Page Group structured field terminates a page group that was initiated by a Begin Named Page Group structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public ENG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}