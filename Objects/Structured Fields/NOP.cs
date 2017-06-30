using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class NOP : StructuredField
    {
        private static string _abbr = "NOP";
        private static string _title = "No Operation";
        private static string _desc = "The data in the No Operation structured field is untyped and undefined. Although not recommended, custom data streams can be utilized in the data field.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Unused data")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public NOP(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
    }
}