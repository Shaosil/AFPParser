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
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public EFN(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
    }
}