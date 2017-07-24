using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class BDT : StructuredField
    {
        private static string _abbr = "BDT";
        private static string _title = "Begin Document";
        private static string _desc = "The Begin Document structured field names and begins the document.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Name"),
            new Offset(8, Lookups.DataTypes.EMPTY, ""),
            new Offset(10, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        private string _docName;
        public string DocumentName
        {
            get { return _docName; }
            private set
            {
                _docName = value;
                PutStringInData(value, 0, 8);
            }
        }

        public BDT(string docName = "") : base(Lookups.StructuredFieldID<BDT>(), 0, 0, null)
        {
            Data = new byte[8];
            DocumentName = docName;
        }

        public BDT(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            base.ParseData();

            _docName = GetReadableDataPiece(0, 8);
        }
    }
}