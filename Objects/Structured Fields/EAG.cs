using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class EAG : StructuredField
    {
        private static string _abbr = "EAG";
        private static string _title = "End Active Environment Group";
        private static string _desc = "Terminates the definition of an active environment group initiated by a BAG field.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Name")
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

        public EAG(string groupName = "") : base(Lookups.StructuredFieldID<EAG>(), 0, 0, null)
        {
            Data = new byte[8];
            GroupName = _groupName;
        }

        public EAG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            base.ParseData();

            _groupName = GetReadableDataPiece(0, 8);
        }
    }
}