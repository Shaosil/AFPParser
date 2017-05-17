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
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public ESG(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}