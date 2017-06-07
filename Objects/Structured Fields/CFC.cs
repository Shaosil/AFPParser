using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class CFC : StructuredField
	{
		private static string _abbr = "CFC";
		private static string _title = "Coded Font Control";
		private static string _desc = "Specifies the length of the repeating group in the CFI field.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "CFI RG Length"),
            new Offset(1, Lookups.DataTypes.EMPTY, ""),
            new Offset(2, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int CFIRGLength { get; private set; }

		public CFC(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }

        public override void ParseData()
        {
            CFIRGLength = Data[0];
        }
    }
}