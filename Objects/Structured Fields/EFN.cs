using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class EFN : StructuredField
    {
        private static string _abbr = "EFN";
        private static string _title = "End Font";
        private static string _desc = "Ends the font character set object.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Font Character Set Name")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        protected override List<Offset> Offsets => _oSets;

        public EFN(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }
    }
}