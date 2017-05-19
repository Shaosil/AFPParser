using System.Collections.Generic;
using System.Linq;

namespace AFPParser.StructuredFields
{
    public class FNM : StructuredField
    {
        private static string _abbr = "FNM";
        private static string _title = "Font Patterns Map";
        private static string _desc = "Describes some characteristics of the font character patterns.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Character Box Width"),
            new Offset(2, Lookups.DataTypes.UBIN, "Character Box Height"),
            new Offset(4, Lookups.DataTypes.UBIN, "Pattern Data Offset")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => true;
        protected override int RepeatingGroupStart => 0;
        protected override int RepeatingGroupLength
        {
            get
            {
                int length = Data.Length;
                FNC control = (FNC)Parser.AfpFile.FirstOrDefault(f => f.GetType() == typeof(FNC));
                if (control != null)
                    length = control.Data[21];

                return length;
            }
        }
        protected override List<Offset> Offsets => _oSets;

		public FNM(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}