using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class IPS : StructuredField
    {
        private static string _abbr = "IPS";
        private static string _title = "Include Page Segment";
        private static string _desc = "References a page segment resource object that is to be presented on the page or overlay presentation space.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Page Segment Name"),
            new Offset(8, Lookups.DataTypes.SBIN, "X Axis Origin"),
            new Offset(11, Lookups.DataTypes.SBIN, "Y Axis Origin"),
            new Offset(14, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public string SegmentName { get; private set; }
        public int XOrigin { get; private set; }
        public int YOrigin { get; private set; }

        public IPS(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            base.ParseData();

            SegmentName = GetReadableDataPiece(0, 8);
            XOrigin = GetNumericValueFromData<int>(8, 3);
            YOrigin = GetNumericValueFromData<int>(11, 3);
        }
    }
}