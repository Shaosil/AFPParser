using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EDM : StructuredField
	{
		private static string _abbr = "EDM";
		private static string _title = "End Data Map";
		private static string _desc = "The End Data Map structured field terminates the Data Map object initiated by a Begin Data Map structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EDM(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}