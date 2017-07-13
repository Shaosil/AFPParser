using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class EPT : StructuredField
    {
        private static string _abbr = "EPT";
        private static string _title = "End Presentation Text";
        private static string _desc = "The End Presentation Text Object structured field terminates the current presentation text object initiated by a Begin Presentation Text Object structured field.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Presentation Text Data Object Name"),
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

        public EPT(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public EPT(string name = "") : base(Lookups.StructuredFieldID<EPT>(), 0, 0, null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Data = new byte[8];
                ObjectName = name;
            }
        }
    }
}