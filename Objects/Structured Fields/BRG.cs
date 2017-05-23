using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BRG : StructuredField
	{
		private static string _abbr = "BRG";
		private static string _title = "Begin Resource Group";
		private static string _desc = "The Begin Resource Group structured field begins a resource group, which becomes the current resource group at the same level in the document hierarchy.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Resource Group Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BRG(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}