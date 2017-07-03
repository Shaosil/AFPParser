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
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EDX(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}