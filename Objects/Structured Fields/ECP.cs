using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class ECP : StructuredField
	{
		private static string _abbr = "ECP";
		private static string _title = "End Code Page";
		private static string _desc = "Ends the Code Page object.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Code Page Name")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public ECP(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}