using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EII : StructuredField
	{
		private static string _abbr = "EII";
		private static string _title = "End Image Object IM";
		private static string _desc = "The End IM Image Object structured field terminates the current IM image object initiated by a Begin IM Image Object structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public EII(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}