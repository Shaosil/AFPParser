using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BMM : StructuredField
	{
		private static string _abbr = "BMM";
		private static string _title = "Begin Medium Map";
		private static string _desc = "Begins a medium map resource object, which is a print control resource object that contains a complete set of controls for presenting pages on physical media.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Medium Map Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BMM(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}