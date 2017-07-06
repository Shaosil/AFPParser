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
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public ERS(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}