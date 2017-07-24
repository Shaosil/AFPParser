using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class EPG : StructuredField
    {
        private static string _abbr = "EPG";
        private static string _title = "End Page";
        private static string _desc = "The End Page structured field terminates the current presentation page definition initiated by a Begin Page structured field.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Page Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        private string _pageName;
        public string PageName
        {
            get { return _pageName; }
            private set
            {
                _pageName = value;
                PutStringInData(value, 0, 8);
            }
        }

        public EPG(string pageName = "") : base(Lookups.StructuredFieldID<EPG>(), 0, 0, null)
        {
            Data = new byte[8];
            PageName = _pageName;
        }

        public EPG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            base.ParseData();

            _pageName = GetReadableDataPiece(0, 8);
        }
    }
}