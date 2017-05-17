using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EDX : StructuredField
	{
		private static string _abbr = "EDX";
		private static string _title = "End Data Map Transmission Subcase";
		private static string _desc = "The End Data Map Transmission Subcase structured field terminates the Data Map Transmission Subcase initiated by a Begin Data Map Transmission Subcase structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public EDX(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}