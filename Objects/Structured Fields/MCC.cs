using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MCC : StructuredField
	{
		private static string _abbr = "MCC";
		private static string _title = "Medium Copy Count";
		private static string _desc = "Specifies the number of copies of each medium, or sheet, to be presented, and the modifications that apply to each copy";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Starting Copy Number"),
            new Offset(2, Lookups.DataTypes.UBIN, "Ending Copy Number"),
            new Offset(4, Lookups.DataTypes.EMPTY, ""),
            new Offset(5, Lookups.DataTypes.UBIN, "MMC ID")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MCC(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}