using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class ESG : StructuredField
	{
		private static string _abbr = "ESG";
		private static string _title = "End Resource Environment Group";
		private static string _desc = "The End Resource Environment Group structured field terminates the definition of a Resource Environment Group initiated by a Begin Resource Environment Group structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public ESG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}