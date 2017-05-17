using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class ERS : StructuredField
	{
		private static string _abbr = "ERS";
		private static string _title = "End Resource";
		private static string _desc = "Terminates an envelope that is used to carry resource objects in print file level resource groups.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CHAR, "Identifier")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public ERS(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}