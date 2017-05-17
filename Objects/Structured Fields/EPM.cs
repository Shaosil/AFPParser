using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EPM : StructuredField
	{
		private static string _abbr = "EPM";
		private static string _title = "End Page Map";
		private static string _desc = "The End Page Map structured field terminates the Page Map object initiated by a Begin Page Map structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public EPM(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}