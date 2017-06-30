using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class BCP : StructuredField
    {
        private static string _abbr = "BCP";
        private static string _title = "Begin Code Page";
        private static string _desc = "Begins a code page object and identifies it by name.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Code Page Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public string ObjectName { get; private set; }

        public BCP(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override void ParseData()
        {
            base.ParseData();

            ObjectName = GetReadableDataPiece(0, 8);
        }
    }
}