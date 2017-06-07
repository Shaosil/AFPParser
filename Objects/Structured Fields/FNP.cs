using System.Linq;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class FNP : StructuredField
	{
		private static string _abbr = "FNP";
		private static string _title = "Font Position";
		private static string _desc = "Describes the common characteristics of all the characters in a font character set.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(2, Lookups.DataTypes.SBIN, "Lowercase Height"),
            new Offset(4, Lookups.DataTypes.SBIN, "Cap-M Height"),
            new Offset(6, Lookups.DataTypes.SBIN, "Max Ascender Height"),
            new Offset(8, Lookups.DataTypes.SBIN, "Max Descender Depth"),
            new Offset(10, Lookups.DataTypes.EMPTY, ""),
            new Offset(17, Lookups.DataTypes.UBIN, "Underscore Width (Units)"),
            new Offset(19, Lookups.DataTypes.UBIN, "Underscore Width (Fraction)"),
            new Offset(20, Lookups.DataTypes.SBIN, "Underscore Position")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => true;
		protected override int RepeatingGroupStart => 0;
        protected override int RepeatingGroupLength
        {
            get
            {
                int length = Data.Length;
                FNC control = LowestLevelContainer.GetStructure<FNC>();
                if (control != null)
                    length = control.Data[20];

                return length;
            }
        }
        public override IReadOnlyList<Offset> Offsets => _oSets;

		public FNP(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}