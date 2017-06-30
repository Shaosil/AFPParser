using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class ERG : StructuredField
	{
		private static string _abbr = "ERG";
		private static string _title = "End Resource Group";
		private static string _desc = "The End Resource Group structured field terminates the definition of a resource group initiated by a Begin Resource Group structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public ERG(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}