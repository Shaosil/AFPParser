using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class BAG : StructuredField
    {
        private static string _abbr = "BAG";
        private static string _title = "Begin Active Environment Group";
        private static string _desc = "Begins an active environment group, which establishes the environment parameters for the page or overlay.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        private string _groupName;
        public string GroupName
        {
            get { return _groupName; }
            private set
            {
                _groupName = value;
                PutStringInData(value, 0, 8);
            }
        }

        public BAG(string groupName = "") : base(Lookups.StructuredFieldID<BAG>(), 0, 0, null)
        {
            Data = new byte[8];
            GroupName = _groupName;
        }

        public BAG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            base.ParseData();

            _groupName = GetReadableDataPiece(0, 8);
        }
    }
}