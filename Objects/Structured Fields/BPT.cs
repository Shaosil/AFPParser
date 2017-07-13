using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class BPT : StructuredField
    {
        private static string _abbr = "BPT";
        private static string _title = "Begin Presentation Text Object";
        private static string _desc = "Begins a presentation text object, which becomes the current data object.";
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
        private string _objectName;
        public string ObjectName
        {
            get { return _objectName; }
            private set
            {
                _objectName = value;
                PutStringInData(value, 0, 8);
            }
        }

        public BPT(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public BPT(string name = "") : base(Lookups.StructuredFieldID<BPT>(), 0, 0, null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Data = new byte[8];
                ObjectName = name;
            }
        }
    }
}